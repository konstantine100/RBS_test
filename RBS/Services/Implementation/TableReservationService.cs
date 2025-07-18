using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Hubs;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Interfaces;
using RBS.Validation;

namespace RBS.Services.Implenetation;

public class TableReservationService : ITableReservationService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IConflictTableService _conflictTableService;
    private readonly IHubContext<RestaurantHub> _hubContext;
    private readonly ILayoutNotificationService _layoutNotificationService;
    
    public TableReservationService(DataContext context, IMapper mapper, IConflictTableService conflictTableService, IHubContext<RestaurantHub> hubContext, ILayoutNotificationService layoutNotificationService)
    {
        _context = context;
        _mapper = mapper;
        _conflictTableService = conflictTableService;
        _hubContext = hubContext;
        _layoutNotificationService = layoutNotificationService;
    }
    
    public async Task<ApiResponse<TableReservationDTO>> ChooseTable(int userId, int tableId, AddReservation request, int? additionalTime)
    {
        var user = await _context.Users
            .Include(x => x.TableReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<TableReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var table = await _context.Tables
                .Include(x => x.TableReservations)
                .FirstOrDefaultAsync(x => x.Id == tableId);

            if (table == null)
            {
                var response = ApiResponseService<TableReservationDTO>
                    .Response(null, "Table not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!table.IsAvailable) 
                {
                    var response = ApiResponseService<TableReservationDTO>
                        .Response(null, "Table is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var reservation = _mapper.Map<TableReservation>(request);
                    reservation.BookingDateEnd = request.BookingDate.AddHours(2);
                    var validator = new TableReservationValidator();
                    var result = validator.Validate(reservation);

                    if (additionalTime != null)
                    {
                        if (additionalTime >= 0 && additionalTime <= 12)
                        {
                            reservation.BookingDateEnd = request.BookingDate.AddHours(additionalTime.Value);
                        }
                        else
                        {
                            var response = ApiResponseService<TableReservationDTO>
                                .Response(null, "wrong additional time", StatusCodes.Status400BadRequest);
                            return response;
                        }
                    }

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<TableReservationDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    
                    else
                    {
                        var allBookingConflicts = await _conflictTableService.ConflictTableBookings(table.SpaceId, tableId, reservation.BookingDate, reservation.BookingDateEnd);
                        var spaceReservationConflicts = await _conflictTableService.ConflictSpaceReservation(table.SpaceId, reservation.BookingDate, reservation.BookingDateEnd);
                        var tableReservationConflicts = await _conflictTableService.ConflictTableReservation(tableId, reservation.BookingDate, reservation.BookingDateEnd);
                        var chairReservationConflicts = await _conflictTableService.ConflictChairReservation(tableId, reservation.BookingDate, reservation.BookingDateEnd);
                        var walkInConflicts = await _conflictTableService.ConflictWalkIn(tableId, reservation.BookingDate);

                        if (allBookingConflicts.Any() || spaceReservationConflicts.Any() || tableReservationConflicts.Any() || chairReservationConflicts.Any() || walkInConflicts.Any())
                        {
                            var response = ApiResponseService<TableReservationDTO>
                                .Response(null, "table not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            reservation.Price = table.TablePrice;
                            reservation.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            reservation.Table = table;
                            user.TableReservations.Add(reservation);
                            table.TableReservations.Add(reservation);
                            await _context.SaveChangesAsync();
                            
                            await _hubContext.Clients.Group($"Space_{table.SpaceId}")
                                .SendAsync("TableReserved", new {
                                    tableId = tableId,
                                    reservationId = reservation.Id,
                                    expiresAt = reservation.BookingExpireDate,
                                    userId = userId
                                });
                            
                            await _hubContext.Clients.Group($"Space_{table.SpaceId}")
                                .SendAsync("LayoutChanged", new {
                                    spaceId = table.SpaceId,
                                    changeType = "TableReservation",
                                    itemId = tableId,
                                    timestamp = DateTime.UtcNow
                                });
                        
                            var response = ApiResponseService<TableReservationDTO>
                                .Response200(_mapper.Map<TableReservationDTO>(reservation));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> TableBookingForDay(int tableId, DateTime date)
    {
        var table = await _context.Tables
            .Include(x => x.Bookings)
            .FirstOrDefaultAsync(x => x.Id == tableId);

        if (table == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "Table not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var bookings = table.Bookings
                .Where(x => x.BookingDate.Date == date.Date)
                .OrderBy(x => x.BookingDate)
                .ToList();
            
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;

        }
    }

    public async Task<ApiResponse<TableReservationDTO>> RemoveReservationTable(int userId, int reservationId)
    {
        var user = await _context.Users
            .Include(x => x.TableReservations)
            .ThenInclude(x => x.Table)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<TableReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var reservation = user.TableReservations.FirstOrDefault(x => x.Id == reservationId);

            if (reservation == null)
            {
                var response = ApiResponseService<TableReservationDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                _context.TableReservations.Remove(reservation);
                await _context.SaveChangesAsync();
                
                // Notify clients about the reservation removal
                await _hubContext.Clients.Group($"Space_{reservation.TableId}")
                    .SendAsync("TableReservationRemoved", new {
                        tableId = reservation.TableId,
                        spaceId = reservation.Table.SpaceId,
                        reservationId = reservationId,
                        userId = userId
                    });
                
                // Notify about layout change
                await _layoutNotificationService.NotifyLayoutChanged(reservation.Table.SpaceId, "TableReservationRemoved", new { 
                    itemId = reservation.TableId, 
                    userId = userId,
                    reservationId = reservationId
                });
                
                var response = ApiResponseService<TableReservationDTO>
                    .Response200(_mapper.Map<TableReservationDTO>(reservation));
                return response;
            }
        }
    }
}