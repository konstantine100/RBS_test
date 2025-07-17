using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Models;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;

namespace RBS.Services.Implementation;

public class WalkInService : IWalkInService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IConflictTableService _conflictTableService;
    private readonly IConflictChairService _conflictChairService;
    
    public WalkInService(DataContext context, IMapper mapper, IConflictTableService conflictTableService, IConflictChairService conflictChairService)
    {
        _context = context;
        _mapper = mapper;
        _conflictTableService = conflictTableService;
        _conflictChairService = conflictChairService;
    }
    
    public async Task<ApiResponse<WalkInDTO>> AddWalkInTable(int hostId, int tableId)
    {
        var host = await _context.Users
            .Include(x => x.AcceptedWalkIns)
            .FirstOrDefaultAsync(x => x.Id == hostId);

        if (host == null)
        {
            var response = ApiResponseService<WalkInDTO>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var table = await _context.Tables
                .Include(x => x.WalkIns)
                .FirstOrDefaultAsync(x => x.Id == tableId);

            if (table == null)
            {
                var response = ApiResponseService<WalkInDTO>
                    .Response(null, "Table not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (!table.IsAvailable) 
                {
                    var response = ApiResponseService<WalkInDTO>
                        .Response(null, "Table is not available", StatusCodes.Status400BadRequest);
                    return response;
                }
                else
                {
                    var allBookingConflicts = await _conflictTableService.ConflictTableBookings(table.SpaceId, tableId, DateTime.UtcNow);
                    var spaceReservationConflicts = await _conflictTableService.ConflictSpaceReservation(table.SpaceId, DateTime.UtcNow);
                    var tableReservationConflicts = await _conflictTableService.ConflictTableReservation(tableId, DateTime.UtcNow);
                    var chairReservationConflicts = await _conflictTableService.ConflictChairReservation(tableId, DateTime.UtcNow);
                    var walkInConflicts = await _conflictTableService.ConflictWalkIn(tableId, DateTime.UtcNow);

                    if (allBookingConflicts.Any() || spaceReservationConflicts.Any() || tableReservationConflicts.Any() || chairReservationConflicts.Any() || walkInConflicts.Any())
                    {
                        var response = ApiResponseService<WalkInDTO>
                            .Response(null, "table not available", StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        WalkIn walkInToAdd = new WalkIn();
                        walkInToAdd.HostId = host.Id;
                        walkInToAdd.TableId = table.Id;
                        walkInToAdd.Table = table;
                        host.AcceptedWalkIns.Add(walkInToAdd);
                        
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<WalkInDTO>
                            .Response200(_mapper.Map<WalkInDTO>(walkInToAdd));
                        return response;
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<WalkInDTO>> AddWalkInChair(int hostId, int chairId)
    {
        var host = await _context.Users
            .Include(x => x.AcceptedWalkIns)
            .FirstOrDefaultAsync(x => x.Id == hostId);

        if (host == null)
        {
            var response = ApiResponseService<WalkInDTO>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var chair = await _context.Chairs
                .Include(x => x.WalkIns)
                .FirstOrDefaultAsync(x => x.Id == chairId);

            if (chair == null)
            {
                var response = ApiResponseService<WalkInDTO>
                    .Response(null, "Chair not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (chair.IsAvailable == false)
                {
                    var response = ApiResponseService<WalkInDTO>
                        .Response(null, "Chair is not available", StatusCodes.Status400BadRequest);
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
                        await _conflictChairService.ConflictChairBookings(chairId, DateTime.UtcNow);
                    var spaceReservationConflicts =
                        await _conflictChairService.ConflictSpaceReservation(space.Id, DateTime.UtcNow);
                    var tableReservationConflicts =
                        await _conflictChairService.ConflictTableReservation(chair.TableId, DateTime.UtcNow);
                    var chairReservationConflicts =
                        await _conflictChairService.ConflictChairReservation(chairId, DateTime.UtcNow);
                    var walkInConflicts =
                        await _conflictChairService.ConflictWalkIn(chairId, DateTime.UtcNow);

                        
                        
                    if (allBookingConflicts.Any() || spaceReservationConflicts.Any() || tableReservationConflicts.Any() || chairReservationConflicts.Any() || walkInConflicts.Any())
                    {
                        var response = ApiResponseService<WalkInDTO>
                            .Response(null, "chair not available", StatusCodes.Status400BadRequest);
                        return response;
                    }
                    else
                    {
                        WalkIn walkInToAdd = new WalkIn();
                        walkInToAdd.HostId = host.Id;
                        walkInToAdd.ChairId = chairId;
                        walkInToAdd.Chair = chair;
                        host.AcceptedWalkIns.Add(walkInToAdd);
                        
                        await _context.SaveChangesAsync();
                        
                        var response = ApiResponseService<WalkInDTO>
                            .Response200(_mapper.Map<WalkInDTO>(walkInToAdd));
                        return response;
                    }
                }
            }
        }
    }

    public async Task<ApiResponse<List<WalkInDTO>>> GetMyWalkIns(int hostId)
    {
        var host = await _context.Users
            .Include(x => x.AcceptedWalkIns)
            .FirstOrDefaultAsync(x => x.Id == hostId);

        if (host == null)
        {
            var response = ApiResponseService<List<WalkInDTO>>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var walkIns = host.AcceptedWalkIns
                .OrderByDescending(x => x.WalkInAt)
                .ToList();
            
            var response = ApiResponseService<List<WalkInDTO>>
                .Response200(_mapper.Map<List<WalkInDTO>>(walkIns));
            return response;
        }
    }
    
    public async Task<ApiResponse<WalkInDTO>> FinishWalkIn(int hostId, int walkInId)
    {
        var host = await _context.Users
            .Include(x => x.AcceptedWalkIns)
            .FirstOrDefaultAsync(x => x.Id == hostId);

        if (host == null)
        {
            var response = ApiResponseService<WalkInDTO>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var walkIn = host.AcceptedWalkIns
                .FirstOrDefault(x => x.Id == walkInId);

            if (walkIn == null)
            {
                var response = ApiResponseService<WalkInDTO>
                    .Response(null, "Walk In not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                walkIn.IsFinished = true;
                await _context.SaveChangesAsync();
                
                var response = ApiResponseService<WalkInDTO>
                    .Response200(_mapper.Map<WalkInDTO>(walkIn));
                return response;
            }
        }
    }
}