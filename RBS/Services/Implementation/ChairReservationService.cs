using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
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
    
    public ChairReservationService(DataContext context, IMapper mapper, IConflictChairService conflictChairService)
    {
        _context = context;
        _mapper = mapper;
        _conflictChairService = conflictChairService;
    }
    
    public async Task<ApiResponse<ChairReservationDTO>> ChooseChair(int userId, int chairId, AddReservation request) 
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
                else
                {
                    var reservation = _mapper.Map<ChairReservation>(request);
                    var validator = new ChairReservationValidator();
                    var result = validator.Validate(reservation);

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
                            await _conflictChairService.ConflictChairBookings(chairId, reservation.BookingDate);
                        var spaceReservationConflicts =
                            await _conflictChairService.ConflictSpaceReservation(space.Id, reservation.BookingDate);
                        var tableReservationConflicts =
                            await _conflictChairService.ConflictTableReservation(chair.TableId, reservation.BookingDate);
                        var chairReservationConflicts =
                            await _conflictChairService.ConflictChairReservation(chairId, reservation.BookingDate);

                        if (allBookingConflicts.Count != 0 || spaceReservationConflicts.Count != 0 || tableReservationConflicts.Count != 0 || chairReservationConflicts.Count != 0)
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
                        
                            var response = ApiResponseService<ChairReservationDTO>
                                .Response200(_mapper.Map<ChairReservationDTO>(reservation));
                            return response;
                        }
                    }
                }
            }
        }
    }
    
    public async Task<ApiResponse<ChairReservationDTO>> RemoveReservationChair(int userId, int reservationId)
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
                
                var response = ApiResponseService<ChairReservationDTO>
                    .Response200(_mapper.Map<ChairReservationDTO>(reservation));
                return response;
            }
        }
    }
}