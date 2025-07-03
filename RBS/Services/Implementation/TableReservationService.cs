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

public class TableReservationService : ITableReservationService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IConflictTableService _conflictTableService;
    
    public TableReservationService(DataContext context, IMapper mapper, IConflictTableService conflictTableService)
    {
        _context = context;
        _mapper = mapper;
        _conflictTableService = conflictTableService;
    }
    
    public async Task<ApiResponse<TableReservationDTO>> ChooseTable(int userId, int tableId, AddReservation request)
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
                    var validator = new TableReservationValidator();
                    var result = validator.Validate(reservation);

                    if (!result.IsValid)
                    {
                        string errors = string.Join(", ", result.Errors.Select(x => x.ErrorMessage));
                        
                        var response = ApiResponseService<TableReservationDTO>
                            .Response(null, errors, StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        var allBookingConflicts = await _conflictTableService.ConflictTableBookings(table.SpaceId, tableId, reservation.BookingDate);
                        var spaceReservationConflicts = await _conflictTableService.ConflictSpaceReservation(table.SpaceId, reservation.BookingDate);
                        var tableReservationConflicts = await _conflictTableService.ConflictTableReservation(tableId, reservation.BookingDate);
                        var chairReservationConflicts = await _conflictTableService.ConflictChairReservation(tableId, reservation.BookingDate);

                        if (allBookingConflicts.Count != 0 || spaceReservationConflicts.Count != 0 || tableReservationConflicts.Count != 0 || chairReservationConflicts.Count != 0)
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
                        
                            var response = ApiResponseService<TableReservationDTO>
                                .Response200(_mapper.Map<TableReservationDTO>(reservation));
                            return response;
                        }
                    }
                }
            }
        }
    }
    
    public async Task<ApiResponse<TableReservationDTO>> RemoveReservationTable(int userId, int reservationId)
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
                
                var response = ApiResponseService<TableReservationDTO>
                    .Response200(_mapper.Map<TableReservationDTO>(reservation));
                return response;
            }
        }
    }
}