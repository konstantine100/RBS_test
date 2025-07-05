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

public class SpaceReservationService : ISpaceReservationService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IConflictSpaceService _conflictSpaceService;
    
    public SpaceReservationService(DataContext context, IMapper mapper, IConflictSpaceService conflictSpaceService)
    {
        _context = context;
        _mapper = mapper;
        _conflictSpaceService = conflictSpaceService;
    }
    
    public async Task<ApiResponse<SpaceReservationDTO>> ChooseSpace(int userId, int spaceId, AddReservation request, DateTime endDate)
    {
        var user = await _context.Users
            .Include(x => x.SpaceReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<SpaceReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var space = await _context.Spaces
                .Include(x => x.SpaceReservations)
                .FirstOrDefaultAsync(x => x.Id == spaceId);

            if (space == null)
            {
                var response = ApiResponseService<SpaceReservationDTO>
                    .Response(null, "Space not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!space.IsAvailable)
                {
                    var response = ApiResponseService<SpaceReservationDTO>
                        .Response(null, "Space is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var reservation = _mapper.Map<SpaceReservation>(request);
                    reservation.BookingDateEnd = endDate;
                    
                    var validator = new SpaceReservationValidator();
                    var result = validator.Validate(reservation);
                    
                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<SpaceReservationDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        var allConflictingBookings = await _conflictSpaceService.ConflictSpaceBookings(spaceId, reservation.BookingDate, endDate);
                        var allConflictingSpaceReservations = await _conflictSpaceService.ConflictSpaceReservation(spaceId, reservation.BookingDate, endDate);
                        var allConflictingTableReservations = await _conflictSpaceService.ConflictSpaceTableReservation(spaceId, reservation.BookingDate, endDate);
                        var allConflictingChairReservations = await _conflictSpaceService.ConflictSpaceChairReservation(spaceId, reservation.BookingDate, endDate);
                        var allConflictingWalkings = await _conflictSpaceService.ConflictWalkIns(spaceId, reservation.BookingDate, endDate);
                        
                        if (allConflictingBookings.Any() || allConflictingSpaceReservations.Any() || allConflictingTableReservations.Any() || allConflictingChairReservations.Any() || allConflictingWalkings.Any())
                        {
                            var response = ApiResponseService<SpaceReservationDTO>
                                .Response(null, "can't book at that time!", StatusCodes.Status400BadRequest);
                            return response;
                        }
                        else
                        {
                            reservation.Price = space.SpacePrice;
                            reservation.BookingExpireDate = DateTime.UtcNow.AddMinutes(10);
                            reservation.Space = space;
                            user.SpaceReservations.Add(reservation);
                            space.SpaceReservations.Add(reservation);
                            
                            await _context.SaveChangesAsync();

                            var response = ApiResponseService<SpaceReservationDTO>
                                .Response200(_mapper.Map<SpaceReservationDTO>(reservation));
                            return response;
                        }
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<SpaceReservationDTO>> RemoveReservationSpace(int userId, int reservationId)
    {
        var user = await _context.Users
            .Include(x => x.SpaceReservations)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var response = ApiResponseService<SpaceReservationDTO>
                .Response(null, "User not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var reservation = user.SpaceReservations.FirstOrDefault(x => x.Id == reservationId);

            if (reservation == null)
            {
                var response = ApiResponseService<SpaceReservationDTO>
                    .Response(null, "Reservation not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                _context.SpaceReservations.Remove(reservation);
                await _context.SaveChangesAsync();
                
                var response = ApiResponseService<SpaceReservationDTO>
                    .Response200(_mapper.Map<SpaceReservationDTO>(reservation));
                return response;
            }
        }
    }
}