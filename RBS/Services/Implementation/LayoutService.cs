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
    
    public LayoutService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    //need to change!!!!!!
    public async Task<ApiResponse<List<LayoutByHour>>> GetLayoutByHour(int spaceId, DateTime Date)
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
        var spaceBooking = ReturnFilteredSpaceBookings(space, Date);
        var tableBookings = ReturnFilteredTableBookings(tables, Date);
        var chairBookings = ReturnFilteredChairBookings(chairs, Date);
        // reserved entities
        var spaceReservation = ReturnFilteredSpaceReservations(space, Date);
        var tableReservation = ReturnFilteredTableReservations(tables, Date);
        var chairReservation = ReturnFilteredChairReservations(chairs, Date);
        // empty entities
        var noTableBookings = await ReturnEmptyTables(spaceId, Date);
        var noChairBookings = await ReturnEmptyChairs(spaceId, Date);

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
                     .Any(x => x.BookingDate.Year == Date.Year &&
                               x.BookingDate.Month == Date.Month &&
                               x.BookingDate.Day == Date.Day &&
                               x.BookingDate.Hour == Date.Hour))
            {
                var layout = new LayoutByHour();
            
                layout.ChairId = chair.Id;
                layout.Chair = _mapper.Map<ChairDTO>(chair);
                layout.Status = AVAILABLE_STATUS.Booked;
                    
                allLayout.Add(layout);
            }
            else if (chair.Table.TableReservations
                     .Any(x => x.BookingDate.Year == Date.Year &&
                               x.BookingDate.Month == Date.Month &&
                               x.BookingDate.Day == Date.Day &&
                               x.BookingDate.Hour == Date.Hour))
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

    private List<Booking> ReturnFilteredSpaceBookings(Space space, DateTime Date)
    {
        var spaceBooking = space.Bookings
            .Where(x => x.BookingDate.Year == Date.Year &&
                        x.BookingDate.Month == Date.Month &&
                        x.BookingDate.Day == Date.Day &&
                        x.BookingDate.Hour == Date.Hour)
            .ToList();
        
        return spaceBooking;
    }
    
    private List<Table> ReturnFilteredTableBookings(List<Table> tables, DateTime Date)
    {
        var tableBookings = tables
            .Where(x => x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour))
            .ToList();
        
        return tableBookings;
    }
    
    private List<Chair> ReturnFilteredChairBookings(List<Chair> chairs, DateTime Date)
    {
        var chairBookings = chairs
            .Where(x => x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour))
            .ToList();
        
        return chairBookings;
    }
    
    private List<SpaceReservation> ReturnFilteredSpaceReservations(Space space, DateTime Date)
    {
        var spaceReservation = space.SpaceReservations
            .Where(x => x.BookingDate.Year == Date.Year &&
                        x.BookingDate.Month == Date.Month &&
                        x.BookingDate.Day == Date.Day &&
                        x.BookingDate.Hour == Date.Hour)
            .ToList();
        
        return spaceReservation;
    }
    
    private List<Table> ReturnFilteredTableReservations(List<Table> tables, DateTime Date)
    {
        var tableReservation = tables
            .Where(x => x.TableReservations
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour))
            .ToList();
        
        return tableReservation;
    }
    
    private List<Chair> ReturnFilteredChairReservations(List<Chair> chairs, DateTime Date)
    {
        var chairReservation = chairs
            .Where(x => x.ChairReservations
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour))
            .ToList();
        
        return chairReservation;
    }

    private async Task<List<Table>> ReturnEmptyTables(int spaceId, DateTime Date)
    {
        var noTableBookings = await _context.Tables
            .Include(x => x.Bookings)
            .Where(x => x.SpaceId == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour))
            .ToListAsync();

        return noTableBookings;
    }
    
    private async Task<List<Chair>> ReturnEmptyChairs(int spaceId, DateTime Date)
    {
        var noChairBookings = await _context.Chairs
            .Include(x => x.Table)
            .Include(x => x.Bookings)
            .Include(x => x.Table.TableReservations)
            .Where(x => x.Table.SpaceId == spaceId && !x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour))
            .ToListAsync();

        return noChairBookings;
    }

    
}