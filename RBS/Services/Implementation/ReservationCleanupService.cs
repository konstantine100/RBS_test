using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RBS.Data;
using RBS.Hubs;

namespace RBS.Services.Implementation;

public class ReservationCleanupService : BackgroundService  
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReservationCleanupService> _logger;
    
    public ReservationCleanupService(IServiceProvider serviceProvider, ILogger<ReservationCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiredReservations();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in reservation cleanup service");
            }
        }
    }
    
    private async Task CheckExpiredReservations()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<RestaurantHub>>();

        // Table Reservations
        var expiredTableReservations = await context.TableReservations
            .Include(r => r.Table)
            .Where(r => r.BookingExpireDate <= DateTime.UtcNow)
            .ToListAsync();

        // Chair Reservations
        var expiredChairReservations = await context.ChairReservations
            .Include(r => r.Chair)
            .ThenInclude(c => c.Table) 
            .Where(r => r.BookingExpireDate <= DateTime.UtcNow)
            .ToListAsync();

        // Space Reservations 
        var expiredSpaceReservations = await context.SpaceReservations
            .Include(r => r.Space) 
            .Where(r => r.BookingExpireDate <= DateTime.UtcNow)
            .ToListAsync();
        
        // Collect affected space IDs for layout updates
        var affectedSpaceIds = new HashSet<int>();

        // Process Table Reservations
        foreach (var reservation in expiredTableReservations)
        {
            context.TableReservations.Remove(reservation);
            affectedSpaceIds.Add(reservation.Table.SpaceId);
            
            await hubContext.Clients.Group($"Space_{reservation.Table.SpaceId}")
                .SendAsync("TableExpired", new {
                    tableId = reservation.Table.Id,
                    reservationId = reservation.Id
                });
        }

        // Process Chair Reservations
        foreach (var reservation in expiredChairReservations)
        {
            context.ChairReservations.Remove(reservation);
            affectedSpaceIds.Add(reservation.Chair.Table.SpaceId);
            
            await hubContext.Clients.Group($"Space_{reservation.Chair.Table.SpaceId}")
                .SendAsync("ChairExpired", new {
                    chairId = reservation.Chair.Id,
                    tableId = reservation.Chair.TableId,
                    reservationId = reservation.Id
                });
        }

        // Process Space Reservations
        foreach (var reservation in expiredSpaceReservations)
        {
            context.SpaceReservations.Remove(reservation);
            affectedSpaceIds.Add(reservation.Space.Id);
            
            await hubContext.Clients.Group($"Space_{reservation.Space.Id}")
                .SendAsync("SpaceExpired", new {
                    spaceId = reservation.Space.Id,
                    reservationId = reservation.Id
                });
        }

        // Save all changes
        var totalExpired = expiredTableReservations.Count + expiredChairReservations.Count + expiredSpaceReservations.Count;
        if (totalExpired > 0)
        {
            await context.SaveChangesAsync();
            
            foreach (var spaceId in affectedSpaceIds)
            {
                await hubContext.Clients.Group($"Space_{spaceId}")
                    .SendAsync("LayoutChanged", new {
                        spaceId = spaceId,
                        changeType = "ReservationExpired",
                        timestamp = DateTime.UtcNow
                    });
            }
            
            _logger.LogInformation($"Deleted {totalExpired} expired reservations (Tables: {expiredTableReservations.Count}, Chairs: {expiredChairReservations.Count}, Spaces: {expiredSpaceReservations.Count})");
        }
        
        
    }
}