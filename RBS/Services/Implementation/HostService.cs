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
    
    public HostService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<ApiResponse<List<BookingDTO>>> GetRestaurantBookings(int restaurantId)
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
                .Where(x => x.RestaurantId == restaurantId)
                .ToListAsync();
            
            var response = ApiResponseService<List<BookingDTO>>
                .Response200(_mapper.Map<List<BookingDTO>>(bookings));
            return response;
        }
    }

    public async Task<ApiResponse<List<LayoutByHour>>> GetCurrentLayout(int spaceId)
    {
        var space = await _context.Spaces
            .Include(x => x.Bookings)
            .Include(x => x.SpaceReservations)
            .FirstOrDefaultAsync(x => x.Id == spaceId);
        
        var tables = await _context.Tables
            .Include(x => x.Bookings)
            .Include(x => x.TableReservations)
            .Where(x => x.SpaceId == spaceId)
            .ToListAsync();
        
        var chairs = await _context.Chairs
            .Include(x => x.Bookings)
            .Include(x => x.ChairReservations)
            .Include(x => x.Table)
            .Where(x => x.Table.SpaceId == spaceId)
            .ToListAsync();
        
        // booking entities
        var spaceBooking = ReturnFilteredSpaceBookings(space);
        var tableBookings = ReturnFilteredTableBookings(tables);
        var chairBookings = ReturnFilteredChairBookings(chairs);
        // reserved entities
        var spaceReservation = ReturnFilteredSpaceReservations(space);
        var tableReservation = ReturnFilteredTableReservations(tables);
        var chairReservation = ReturnFilteredChairReservations(chairs);
        // empty entities
        var noTableBookings = await ReturnEmptyTables(spaceId);
        var noChairBookings = await ReturnEmptyChairs(spaceId);

        List<LayoutByHour> allLayout = new List<LayoutByHour>();
        
        // bookings
        foreach (var booking in spaceBooking)
        {
            var layout = new LayoutByHour();
            
            layout.SpaceId = spaceId;
            layout.Space = _mapper.Map<SpaceDTO>(space);
            layout.Status = AVAILABLE_STATUS.Booked;
            allLayout.Add(layout);
            
        }

        foreach (var table in tableBookings)
        {
            foreach (var booking in table.Bookings)
            {
                var layout = new LayoutByHour();
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Booked;
                allLayout.Add(layout);
            }
            
        }
        
        foreach (var chair in chairBookings)
        {
            foreach (var booking in chair.Bookings)
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Booked;
                allLayout.Add(layout);
            }
        }
        
        // reservations
        foreach (var reservation in spaceReservation)
        {
            var layout = new LayoutByHour();
            
            layout.SpaceId = spaceId;
            layout.Space = _mapper.Map<SpaceDTO>(space);
            layout.Status = AVAILABLE_STATUS.Reserved;
            allLayout.Add(layout);
            
        }

        foreach (var table in tableReservation)
        {
            foreach (var reservation in table.TableReservations)
            {
                var layout = new LayoutByHour();
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Reserved;
                allLayout.Add(layout);
            }
            
        }
        
        foreach (var chair in chairReservation)
        {
            foreach (var reservation in chair.ChairReservations)
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Reserved;
                allLayout.Add(layout);
            }
        }

        // empty
        if (!spaceBooking.Any() && !spaceReservation.Any())
        {
            var notBookedSpaceLayout = new LayoutByHour
            {
                SpaceId = spaceId,
                Space = _mapper.Map<SpaceDTO>(space),
                Status = AVAILABLE_STATUS.None,
            };
            allLayout.Add(notBookedSpaceLayout);
        }
        
        foreach (var table in noTableBookings)
        {
            if (spaceBooking.Any())
            {
                var layout = new LayoutByHour();
            
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
                allLayout.Add(layout);
            }
            else if (spaceReservation.Any())
            {
                var layout = new LayoutByHour();
            
                layout.TableId = table.Id;
                layout.Table = _mapper.Map<TableDTO>(table);
                layout.Status = AVAILABLE_STATUS.Reserved;
                    
                allLayout.Add(layout);
            }
            else
            {
                var layout = new LayoutByHour
                {
                    TableId = table.Id,
                    Table = _mapper.Map<TableDTO>(table),
                    Status = AVAILABLE_STATUS.None
                };
                
                allLayout.Add(layout);
            }
            
        }
        
        foreach (var chair in noChairBookings)
        {
            if (spaceBooking.Any())
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
                allLayout.Add(layout);
            }
            else if (spaceReservation.Any())
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Reserved;
                    
                allLayout.Add(layout);
            }
            else if (chair.Table.Bookings
                     .Any(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                               x.BookingDate.Month == DateTime.UtcNow.Month &&
                               x.BookingDate.Day == DateTime.UtcNow.Day &&
                               x.BookingDate.Hour == DateTime.UtcNow.Hour))
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
                allLayout.Add(layout);
            }
            else if (chair.Table.TableReservations
                     .Any(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                               x.BookingDate.Month == DateTime.UtcNow.Month &&
                               x.BookingDate.Day == DateTime.UtcNow.Day &&
                               x.BookingDate.Hour == DateTime.UtcNow.Hour))
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Reserved;
                    
                allLayout.Add(layout);
            }
            else
            {
                var layout = new LayoutByHour
                {
                    ChairId = chair.Id,
                    Chair = _mapper.Map<ChairDTO>(chair),
                    Status = AVAILABLE_STATUS.None,
                };
                
                allLayout.Add(layout);
            }
            
        }
        
        //response
        var response = ApiResponseService<List<LayoutByHour>>
            .Response200(allLayout);
        return response;
    }

    public async Task<ApiResponse<BookingDTO>> UpdateBookingLateTimes(int bookingId, TimeSpan lateTime)
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
            if (lateTime.Minutes < 15 || lateTime.Minutes > 5)
            {
                var response = ApiResponseService<BookingDTO>
                    .Response(null, "incorrect time!", StatusCodes.Status400BadRequest);
                return response;
            }
            else
            {
                booking.BookingDateExpiration += lateTime;
                await _context.SaveChangesAsync();
                
                var response = ApiResponseService<BookingDTO>
                    .Response200(_mapper.Map<BookingDTO>(booking));
                return response;
            }
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
            booking.IsFinished = true;
            await _context.SaveChangesAsync();
            
            var response = ApiResponseService<BookingDTO>
                .Response200(_mapper.Map<BookingDTO>(booking));
            return response;
        }
    }
    
    
    private List<Booking> ReturnFilteredSpaceBookings(Space space)
    {
        var spaceBooking = space.Bookings
            .Where(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                        x.BookingDate.Month == DateTime.UtcNow.Month &&
                        x.BookingDate.Day == DateTime.UtcNow.Day &&
                        x.BookingDate.Hour == DateTime.UtcNow.Hour)
            .ToList();
        
        return spaceBooking;
    }
    
    private List<Table> ReturnFilteredTableBookings(List<Table> tables)
    {
        var tableBookings = tables
            .Where(x => x.Bookings
                .Any(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                          x.BookingDate.Month == DateTime.UtcNow.Month &&
                          x.BookingDate.Day == DateTime.UtcNow.Day &&
                          x.BookingDate.Hour == DateTime.UtcNow.Hour))
            .ToList();
        
        return tableBookings;
    }
    
    private List<Chair> ReturnFilteredChairBookings(List<Chair> chairs)
    {
        var chairBookings = chairs
            .Where(x => x.Bookings
                .Any(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                          x.BookingDate.Month == DateTime.UtcNow.Month &&
                          x.BookingDate.Day == DateTime.UtcNow.Day &&
                          x.BookingDate.Hour == DateTime.UtcNow.Hour))
            .ToList();
        
        return chairBookings;
    }
    
    private List<SpaceReservation> ReturnFilteredSpaceReservations(Space space)
    {
        var spaceReservation = space.SpaceReservations
            .Where(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                        x.BookingDate.Month == DateTime.UtcNow.Month &&
                        x.BookingDate.Day == DateTime.UtcNow.Day &&
                        x.BookingDate.Hour == DateTime.UtcNow.Hour)
            .ToList();
        
        return spaceReservation;
    }
    
    private List<Table> ReturnFilteredTableReservations(List<Table> tables)
    {
        var tableReservation = tables
            .Where(x => x.TableReservations
                .Any(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                          x.BookingDate.Month == DateTime.UtcNow.Month &&
                          x.BookingDate.Day == DateTime.UtcNow.Day &&
                          x.BookingDate.Hour == DateTime.UtcNow.Hour))
            .ToList();
        
        return tableReservation;
    }
    
    private List<Chair> ReturnFilteredChairReservations(List<Chair> chairs)
    {
        var chairReservation = chairs
            .Where(x => x.ChairReservations
                .Any(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                          x.BookingDate.Month == DateTime.UtcNow.Month &&
                          x.BookingDate.Day == DateTime.UtcNow.Day &&
                          x.BookingDate.Hour == DateTime.UtcNow.Hour))
            .ToList();
        
        return chairReservation;
    }

    private async Task<List<Table>> ReturnEmptyTables(int spaceId)
    {
        var noTableBookings = await _context.Tables
            .Include(x => x.Bookings)
            .Where(x => x.SpaceId == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                          x.BookingDate.Month == DateTime.UtcNow.Month &&
                          x.BookingDate.Day == DateTime.UtcNow.Day &&
                          x.BookingDate.Hour == DateTime.UtcNow.Hour))
            .ToListAsync();

        return noTableBookings;
    }
    
    private async Task<List<Chair>> ReturnEmptyChairs(int spaceId)
    {
        var noChairBookings = await _context.Chairs
            .Include(x => x.Table)
            .Include(x => x.Bookings)
            .Include(x => x.Table.TableReservations)
            .Where(x => x.Table.SpaceId == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == DateTime.UtcNow.Year &&
                          x.BookingDate.Month == DateTime.UtcNow.Month &&
                          x.BookingDate.Day == DateTime.UtcNow.Day &&
                          x.BookingDate.Hour == DateTime.UtcNow.Hour))
            .ToListAsync();

        return noChairBookings;
    }

}