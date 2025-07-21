using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Hubs;
using RBS.Models;
using RBS.Requests;
using RBS.Services.Interfaces;
using RBS.Validation;

namespace RBS.Services.Implenetation;

public class ChairReservationService : IChairReservationService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IConflictChairService _conflictChairService;
    private readonly IHubContext<RestaurantHub> _hubContext;
    private readonly ILayoutNotificationService _layoutNotificationService;
    
    public ChairReservationService(DataContext context, IMapper mapper, IConflictChairService conflictChairService, IHubContext<RestaurantHub> hubContext, ILayoutNotificationService layoutNotificationService)
    {
        _context = context;
        _mapper = mapper;
        _conflictChairService = conflictChairService;
        _hubContext = hubContext;
        _layoutNotificationService = layoutNotificationService;
    }
    
    public async Task<ApiResponse<ChairReservationDTO>> ChooseChair(int userId, int chairId, AddReservation request, int? additionalTime) 
    {
        var user = await _context.Users
            .Include(x => x.ChairReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ChairReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var chair = await _context.Chairs
                .Include(x => x.Table)
                .Include(x => x.ChairReservations)
                .FirstOrDefaultAsync(x => x.Id == chairId);

            if (chair == null)
            {
                var response = ApiResponseService<ChairReservationDTO>
                    .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (chair.IsAvailable == false)
                {
                    var response = ApiResponseService<ChairReservationDTO>
                        .Response(null, "Chair is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else if (chair.Table.TableType == TABLE_TYPE.VIP || chair.Table.TableType == TABLE_TYPE.REGULAR)
                {
                    var response = ApiResponseService<ChairReservationDTO>
                        .Response(null, "Vip tables chairs is not for booking", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var reservation = _mapper.Map<ChairReservation>(request);
                    reservation.BookingDateEnd = reservation.BookingDate.AddHours(2);
                    var validator = new ChairReservationValidator();
                    var result = validator.Validate(reservation);
                    
                    if (additionalTime != null)
                    {
                        if (additionalTime >= 0 && additionalTime <= 12)
                        {
                            reservation.BookingDateEnd = reservation.BookingDate.AddHours(2 + additionalTime.Value);
                        }
                        else
                        {
                            var response = ApiResponseService<ChairReservationDTO>
                                .Response(null, "wrong additional time", StatusCodes.Status400BadRequest);
                            return response;
                        }
                    }

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<ChairReservationDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        var space = await _context.Spaces
                            .Include(x => x.Tables)
                            .ThenInclude(x => x.Chairs)
                            .FirstOrDefaultAsync(x => x.Tables
                                .Any(x => x.Chairs
                                    .Any(x => x.Id == chairId)));

                        var allBookingConflicts =
                            await _conflictChairService.ConflictChairBookings(chairId, reservation.BookingDate, reservation.BookingDateEnd);
                        var spaceReservationConflicts =
                            await _conflictChairService.ConflictSpaceReservation(space.Id, reservation.BookingDate, reservation.BookingDateEnd);
                        var tableReservationConflicts =
                            await _conflictChairService.ConflictTableReservation(chair.TableId, reservation.BookingDate, reservation.BookingDateEnd);
                        var chairReservationConflicts =
                            await _conflictChairService.ConflictChairReservation(chairId, reservation.BookingDate, reservation.BookingDateEnd);
                        var walkInConflicts =
                            await _conflictChairService.ConflictWalkIn(chairId, reservation.BookingDate);

                        
                        
                        if (allBookingConflicts.Any() || spaceReservationConflicts.Any() || tableReservationConflicts.Any() || chairReservationConflicts.Any() || walkInConflicts.Any())
                        {
                            var response = ApiResponseService<ChairReservationDTO>
                                .Response(null, "chair not available", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            reservation.Price = chair.ChairPrice;
                            reservation.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            reservation.Chair = chair;
                            user.ChairReservations.Add(reservation);
                            chair.ChairReservations.Add(reservation);
                            await _context.SaveChangesAsync();
                            
                            await _hubContext.Clients.Group($"Space_{space.Id}")
                                .SendAsync("TableReserved", new {
                                    chairId = chairId,
                                    reservationId = reservation.Id,
                                    expiresAt = reservation.BookingExpireDate,
                                    userId = userId
                                });
                            
                            await _hubContext.Clients.Group($"Space_{space.Id}")
                                .SendAsync("LayoutChanged", new {
                                    spaceId = space.Id,
                                    changeType = "ChairReservation",
                                    itemId = chairId,
                                    timestamp = DateTime.UtcNow
                                });
                        
                            var response = ApiResponseService<ChairReservationDTO>
                                .Response200(_mapper.Map<ChairReservationDTO>(reservation));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> ChairBookingForDay(int chairId, DateTime date)
    {
        var chair = await _context.Chairs
            .Include(x => x.Bookings)
            .FirstOrDefaultAsync(x => x.Id == chairId);

        if (chair == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "Chair not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var bookings = chair.Bookings
                .Where(x => x.BookingDate.Date == date.Date)
                .OrderBy(x => x.BookingDate)
                .ToList();
            
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;

        }
    }

    public async Task<ApiResponse<ChairReservationDTO>> RemoveReservationChair(int userId, int reservationId)
    {
        var user = await _context.Users
            .Include(x => x.ChairReservations)
            .ThenInclude(x => x.Chair)
            .ThenInclude(x => x.Table)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<ChairReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var reservation = user.ChairReservations.FirstOrDefault(x => x.Id == reservationId);

            if (reservation == null)
            {
                var response = ApiResponseService<ChairReservationDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                _context.ChairReservations.Remove(reservation);
                await _context.SaveChangesAsync();
                
                // Notify clients about the reservation removal
                await _hubContext.Clients.Group($"Space_{reservation.Chair.Table.SpaceId}")
                    .SendAsync("ChairReservationRemoved", new {
                        chairId = reservation.Chair.Id,
                        tableId = reservation.Chair.TableId,
                        spaceId = reservation.Chair.Table.SpaceId,
                        reservationId = reservationId,
                        userId = userId
                    });
                
                // Notify about layout change
                await _layoutNotificationService.NotifyLayoutChanged(reservation.Chair.Table.SpaceId, "ChairReservationRemoved", new { 
                    itemId = reservation.Chair.Id, 
                    userId = userId,
                    reservationId = reservationId
                });
                
                var response = ApiResponseService<ChairReservationDTO>
                    .Response200(_mapper.Map<ChairReservationDTO>(reservation));
                return response;
            }
        }
    }
}