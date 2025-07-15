using System.Runtime.InteropServices.JavaScript;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Models;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;

namespace RBS.Services.Implementation;

public class HostService : IHostService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly ILayoutHelperService _layoutHelperService;
    
    public HostService(DataContext context, IMapper mapper, ILayoutHelperService layoutService)
    {
        _context = context;
        _mapper = mapper;
        _layoutHelperService = layoutService;
    }
    
    public async Task<ApiResponse<List<BookingDTO>>> GetRestaurantCurrentBookings(int restaurantId)
    {
        var restaurant = _context.Restaurants
            .FirstOrDefault(x => x.Id == restaurantId);
    
        if (restaurant == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var bookings = await _context.Bookings
                .Include(x => x.Spaces)
                .Include(x => x.Tables)
                .Include(x => x.Chairs)
                .Include(x => x.User)
                .Where(x => x.RestaurantId == restaurantId && x.BookingStatus == BOOKING_STATUS.Waiting)
                .OrderBy(x => x.BookingDate)
                .ToListAsync();
            
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> GetRestaurantFinishedBookings(int restaurantId)
    {
        var restaurant = _context.Restaurants
            .FirstOrDefault(x => x.Id == restaurantId);
    
        if (restaurant == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var bookings = await _context.Bookings
                .Include(x => x.Spaces)
                .Include(x => x.Tables)
                .Include(x => x.Chairs)
                .Include(x => x.User)
                .Where(x => x.RestaurantId == restaurantId && x.BookingStatus == BOOKING_STATUS.Finished)
                .OrderBy(x => x.BookingDate)
                .ToListAsync();
            
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> GetRestaurantAnnouncedBookings(int restaurantId)
    {
        var restaurant = _context.Restaurants
            .FirstOrDefault(x => x.Id == restaurantId);
    
        if (restaurant == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var bookings = await _context.Bookings
                .Include(x => x.Spaces)
                .Include(x => x.Tables)
                .Include(x => x.Chairs)
                .Include(x => x.User)
                .Where(x => x.RestaurantId == restaurantId && x.BookingStatus == BOOKING_STATUS.Announced)
                .OrderBy(x => x.BookingDate)
                .ToListAsync();
            
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<List<BookingDTO>>> GetRestaurantNotAnnouncedBookings(int restaurantId)
    {
        var restaurant = _context.Restaurants
            .FirstOrDefault(x => x.Id == restaurantId);
    
        if (restaurant == null)
        {
            var response = ApiResponseService<List<BookingDTO>>
                .Response(null, "Restaurant not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var bookings = await _context.Bookings
                .Include(x => x.Spaces)
                .Include(x => x.Tables)
                .Include(x => x.Chairs)
                .Include(x => x.User)
                .Where(x => x.RestaurantId == restaurantId && x.BookingStatus == BOOKING_STATUS.Not_Announced)
                .OrderBy(x => x.BookingDate)
                .ToListAsync();
            
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<List<LayoutByHour>>> GetCurrentLayout(int spaceId)
    {
        var nowTime = DateTime.UtcNow;
        var targetDate = nowTime.Date;
        var targetHour = nowTime.Hour;
        
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
        var spaceBooking = _layoutHelperService.ReturnFilteredSpaceBookings(space, nowTime);
        var tableBookings = _layoutHelperService.ReturnFilteredTableBookings(tables, nowTime);
        var chairBookings = _layoutHelperService.ReturnFilteredChairBookings(chairs, nowTime);
        // reserved entities
        var spaceReservation = _layoutHelperService.ReturnFilteredSpaceReservations(space, nowTime);
        var tableReservation = _layoutHelperService.ReturnFilteredTableReservations(tables, nowTime);
        var chairReservation = _layoutHelperService.ReturnFilteredChairReservations(chairs, nowTime);
        // walkIn entities
        var tableWalkIns = _layoutHelperService.ReturnFilteredTableWalkIn(tables, nowTime);
        var chairWalkIns = _layoutHelperService.ReturnFilteredChairWalkIn(chairs, nowTime);
        // empty entities
        var noTableBookings = await _layoutHelperService.ReturnEmptyTables(spaceId, nowTime);
        var noChairBookings = await _layoutHelperService.ReturnEmptyChairs(spaceId, nowTime);

        List<LayoutByHour> allLayout = new List<LayoutByHour>();
        
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

    public async Task<ApiResponse<BookingDTO>> UpdateBookingLateTimes(int bookingId, int lateTime)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "Booking not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            if (lateTime > 15 || lateTime < 5)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "incorrect time!", StatusCodes.Status400BadRequest);
                return response;
            }
            else
            {
                booking.BookingDateExpiration += TimeSpan.FromMinutes(lateTime);
                await _context.SaveChangesAsync();
                
                var response = ApiResponseService<BookingDTO>
                    .Response200(_mapper.Map<BookingDTO>(booking));
                return response;
            }
        }
    }

    public async Task<ApiResponse<BookingDTO>> BookingUserAnnounced(int bookingId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "Booking not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            booking.BookingStatus = BOOKING_STATUS.Announced;
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<BookingDTO>
                .Response200(_mapper.Map<BookingDTO>(booking));
            return response;
        }
    }

    public async Task<ApiResponse<BookingDTO>> BookingUserNotAnnounced(int bookingId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "Booking not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            booking.BookingStatus = BOOKING_STATUS.Not_Announced;
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<BookingDTO>
                .Response200(_mapper.Map<BookingDTO>(booking));
            return response;
        }
    }

    public async Task<ApiResponse<BookingDTO>> FinishBooking(int bookingId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking == null)
        {
            var response = ApiResponseService<BookingDTO>
                .Response(null, "Booking not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            booking.BookingStatus = BOOKING_STATUS.Finished;
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<BookingDTO>
                .Response200(_mapper.Map<BookingDTO>(booking));
            return response;
        }
    }

    public async Task<ApiResponse<TableDTO>> TableAvailabilityChange(int hostId, int tableId)
    {
        var host = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == hostId);

        if (host == null)
        {
            var response = ApiResponseService<TableDTO>
                .Response(null, "Host not found", StatusCodes.Status404NotFound);
            return response;
        }
        else
        {
            var table = await _context.Tables
                .Include(x => x.Space)
                .FirstOrDefaultAsync(x => x.Id == tableId && x.Space.RestaurantId == host.RestaurantId);

            if (table == null)
            {
                var response = ApiResponseService<TableDTO>
                    .Response(null, "Table not found", StatusCodes.Status404NotFound);
                return response;
            }
            else
            {
                if (table.IsAvailable)
                {
                    table.IsAvailable = false;
                    await _context.SaveChangesAsync();
                    
                }
                else
                {
                    table.IsAvailable = true;
                    await _context.SaveChangesAsync();
                }
                
                var response = ApiResponseService<TableDTO>
                    .Response200(_mapper.Map<TableDTO>(table));
                return response;
            }
        }
    }
    
}