using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Services.Implenetation;

public class LayoutService : ILayoutService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly ILayoutHelperService _layoutHelperService;
    
    public LayoutService(DataContext context, IMapper mapper, ILayoutHelperService layoutService)
    {
        _context = context;
        _mapper = mapper;
        _layoutHelperService = layoutService;
    }
    
    public async Task<ApiResponse<List<LayoutByHour>>> GetLayoutByHour(int spaceId, DateTime Date)
    {
        var space = await _context.Spaces
            .Include(x => x.Bookings)
            .Include(x => x.SpaceReservations)
            .FirstOrDefaultAsync(x => x.Id == spaceId);

        if (space == null)
        {
            var response = ApiResponseService<List<LayoutByHour>>
                .Response(null, "space not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var tables = await _context.Tables
            .Include(x => x.Bookings)
            .Include(x => x.TableReservations)
            .Include(x => x.WalkIns)
            .Where(x => x.SpaceId == spaceId)
            .ToListAsync();
        
        var chairs = await _context.Chairs
            .Include(x => x.Bookings)
            .Include(x => x.ChairReservations)
            .Include(x => x.WalkIns)
            .Include(x => x.Table)
            .Where(x => x.Table.SpaceId == spaceId)
            .ToListAsync();
        
        // booking entities
        var spaceBooking = _layoutHelperService.ReturnFilteredSpaceBookings(space, Date);
        var tableBookings = _layoutHelperService.ReturnFilteredTableBookings(tables, Date);
        var chairBookings = _layoutHelperService.ReturnFilteredChairBookings(chairs, Date);
        // reserved entities
        var spaceReservation = _layoutHelperService.ReturnFilteredSpaceReservations(space, Date);
        var tableReservation = _layoutHelperService.ReturnFilteredTableReservations(tables, Date);
        var chairReservation = _layoutHelperService.ReturnFilteredChairReservations(chairs, Date);
        // walkIn entities
        var tableWalkIns = _layoutHelperService.ReturnFilteredTableWalkIn(tables, Date);
        var chairWalkIns = _layoutHelperService.ReturnFilteredChairWalkIn(chairs, Date);
        // empty entities
        var noTableBookings = await _layoutHelperService.ReturnEmptyTables(spaceId, Date);
        var noChairBookings = await _layoutHelperService.ReturnEmptyChairs(spaceId, Date);

        List<LayoutByHour> allLayout = new List<LayoutByHour>();
        
        var targetDate = Date.Date;
        var targetHour = Date.Hour;
        
        // bookings
        allLayout.AddRange(_layoutHelperService.CreateSpaceBookingLayouts(spaceBooking, spaceId, space));
        allLayout.AddRange(_layoutHelperService.CreateTableBookingLayouts(tableBookings, targetDate, targetHour));
        allLayout.AddRange(_layoutHelperService.CreateChairBookingLayouts(chairBookings, targetDate, targetHour));
        
        // reservations
        allLayout.AddRange(_layoutHelperService.CreateSpaceReservationLayouts(spaceReservation, spaceId, space));
        allLayout.AddRange(_layoutHelperService.CreateTableReservationLayouts(tableReservation, targetDate, targetHour));
        allLayout.AddRange(_layoutHelperService.CreateChairReservationLayouts(chairReservation, targetDate, targetHour));
        
        // walkIns
        allLayout.AddRange(_layoutHelperService.CreateTableWalkInLayouts(tableWalkIns, targetDate, targetHour));
        allLayout.AddRange(_layoutHelperService.CreateChairWalkInLayouts(chairWalkIns, targetDate, targetHour));

        // empty
        if (!spaceBooking.Any() && !spaceReservation.Any())
        {
            var notBookedSpaceLayout = new LayoutByHour
            {
                SpaceId = spaceId,
                Space = _mapper.Map<LayoutSpaceDTO>(space),
                Status = AVAILABLE_STATUS.None,
            };
            allLayout.Add(notBookedSpaceLayout);
        }
        allLayout.AddRange(_layoutHelperService.CreateEmptyTableLayouts(noTableBookings, spaceBooking, spaceReservation));
        allLayout.AddRange(_layoutHelperService.CreateEmptyChairLayouts(noChairBookings, spaceBooking, spaceReservation, targetDate, targetHour));
        
        //response
        var response = ApiResponseService<List<LayoutByHour>>
            .Response200(allLayout);
        return response;
        }
    }
    
}