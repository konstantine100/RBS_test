using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.CORE;
using RBS.Data;
using RBS.DTOs;
using RBS.Enums;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Services.Implementation;

public class LayoutHelperService : ILayoutHelperService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public LayoutHelperService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public List<Booking> ReturnFilteredSpaceBookings(Space space, DateTime Date)
    {
        var spaceBooking = space.Bookings
            .Where(x => x.BookingDate.Year == Date.Year &&
                        x.BookingDate.Month == Date.Month &&
                        x.BookingDate.Day == Date.Day &&
                        x.BookingDate.Hour == Date.Hour &&
                        (x.BookingStatus == BOOKING_STATUS.Waiting ||
                         x.BookingStatus == BOOKING_STATUS.Announced))
            .ToList();
        
        return spaceBooking;
    }

    public List<Table> ReturnFilteredTableBookings(List<Table> tables, DateTime Date)
    {
        var tableBookings = tables
            .Where(x => x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour &&
                          (x.BookingStatus == BOOKING_STATUS.Waiting ||
                           x.BookingStatus == BOOKING_STATUS.Announced)
                          ))
            .ToList();
        
        return tableBookings;
    }

    public List<Chair> ReturnFilteredChairBookings(List<Chair> chairs, DateTime Date)
    {
        var chairBookings = chairs
            .Where(x => x.Bookings
                .Any(x => x.BookingDate.Year == Date.Year &&
                          x.BookingDate.Month == Date.Month &&
                          x.BookingDate.Day == Date.Day &&
                          x.BookingDate.Hour == Date.Hour &&
                          (x.BookingStatus == BOOKING_STATUS.Waiting ||
                           x.BookingStatus == BOOKING_STATUS.Announced)))
            .ToList();
        
        return chairBookings;
    }

    public List<SpaceReservation> ReturnFilteredSpaceReservations(Space space, DateTime Date)
    {
        var spaceReservation = space.SpaceReservations
            .Where(x => x.BookingDate.Year == Date.Year &&
                        x.BookingDate.Month == Date.Month &&
                        x.BookingDate.Day == Date.Day &&
                        x.BookingDate.Hour == Date.Hour)
            .ToList();
        
        return spaceReservation;
    }

    public List<Table> ReturnFilteredTableReservations(List<Table> tables, DateTime Date)
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

    public List<Chair> ReturnFilteredChairReservations(List<Chair> chairs, DateTime Date)
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

    public List<Table> ReturnFilteredTableWalkIn(List<Table> tables, DateTime Date)
    {
        var tableWalkIn = tables
            .Where(x => x.WalkIns
                .Any(x => x.WalkInAt.Year == Date.Year &&
                          x.WalkInAt.Month == Date.Month &&
                          x.WalkInAt.Day == Date.Day &&
                          x.WalkInAt.Hour == Date.Hour &&
                          x.IsFinished == false))
            .ToList();
        
        return tableWalkIn;
    }

    public List<Chair> ReturnFilteredChairWalkIn(List<Chair> chairs, DateTime Date)
    {
        var chairWalkIn = chairs
            .Where(x => x.WalkIns
                .Any(x => x.WalkInAt.Year == Date.Year &&
                          x.WalkInAt.Month == Date.Month &&
                          x.WalkInAt.Day == Date.Day &&
                          x.WalkInAt.Hour == Date.Hour &&
                          x.IsFinished == false))
            .ToList();
        
        return chairWalkIn;
    }

    public async Task<List<Table>> ReturnEmptyTables(int spaceId, DateTime Date)
    {
        var targetDate = Date.Date;
        var targetHour = Date.Hour;
        
        var noTableBookings = await _context.Tables
            .Include(x => x.Bookings)
            .Include(x => x.TableReservations)
            .Include(x => x.WalkIns)
            .Where(x => x.SpaceId == spaceId &&
                        !x.Bookings.Any(b => b.BookingDate.Date == targetDate && b.BookingDate.Hour == targetHour) &&
                        !x.TableReservations.Any(r => r.BookingDate.Date == targetDate && r.BookingDate.Hour == targetHour) &&
                        !x.WalkIns.Any(w => w.WalkInAt.Date == targetDate && w.WalkInAt.Hour == targetHour))
            .ToListAsync();

        return noTableBookings;
    }

    public async Task<List<Chair>> ReturnEmptyChairs(int spaceId, DateTime Date)
    {
        var targetDate = Date.Date;
        var targetHour = Date.Hour;

        var noChairBookings = await _context.Chairs
            .Include(x => x.Table)
            .Include(x => x.Bookings)
            .Include(x => x.WalkIns)
            .Include(x => x.ChairReservations)
            .Include(x => x.Table.TableReservations)
            .Include(x => x.Table.Bookings)
            .Include(x => x.Table.WalkIns)
            .Where(x => x.Table.SpaceId == spaceId &&
                        !x.Bookings.Any(b => b.BookingDate.Date == targetDate && b.BookingDate.Hour == targetHour) &&
                        !x.ChairReservations.Any(r => r.BookingDate.Date == targetDate && r.BookingDate.Hour == targetHour) &&
                        !x.WalkIns.Any(w => w.WalkInAt.Date == targetDate && w.WalkInAt.Hour == targetHour))
            .ToListAsync();

        return noChairBookings;
    }
    
    public List<LayoutByHour> CreateSpaceBookingLayouts(IEnumerable<object> spaceBookings, int spaceId, Space space)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var booking in spaceBookings)
        {
            var layout = new LayoutByHour
            {
                SpaceId = spaceId,
                Space = _mapper.Map<LayoutSpaceDTO>(space),
                Status = AVAILABLE_STATUS.Booked
            };
            layouts.Add(layout);
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateTableBookingLayouts(IEnumerable<Table> tableBookings, DateTime targetDate, int targetHour)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var table in tableBookings)
        {
            var filteredBookings = table.Bookings
                .Where(x => x.BookingDate.Date == targetDate && x.BookingDate.Hour == targetHour)
                .ToList();

            foreach (var booking in filteredBookings)
            {
                var layout = new LayoutByHour
                {
                    TableId = table.Id,
                    Table = _mapper.Map<LayoutTableDTO>(table),
                    Status = AVAILABLE_STATUS.Booked
                };
                layouts.Add(layout);
            }
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateChairBookingLayouts(IEnumerable<Chair> chairBookings, DateTime targetDate, int targetHour)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var chair in chairBookings)
        {
            var filteredBookings = chair.Bookings
                .Where(x => x.BookingDate.Date == targetDate && x.BookingDate.Hour == targetHour)
                .ToList();

            foreach (var booking in filteredBookings)
            {
                var layout = new LayoutByHour
                {
                    ChairId = chair.Id,
                    Chair = _mapper.Map<ChairDTO>(chair),
                    Status = AVAILABLE_STATUS.Booked
                };
                layouts.Add(layout);
            }
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateSpaceReservationLayouts(IEnumerable<object> spaceReservations, int spaceId, Space space)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var reservation in spaceReservations)
        {
            var layout = new LayoutByHour
            {
                SpaceId = spaceId,
                Space = _mapper.Map<LayoutSpaceDTO>(space),
                Status = AVAILABLE_STATUS.Reserved
            };
            layouts.Add(layout);
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateTableReservationLayouts(IEnumerable<Table> tableReservations, DateTime targetDate, int targetHour)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var table in tableReservations)
        {
            var filteredReservations = table.TableReservations
                .Where(x => x.BookingDate.Date == targetDate && x.BookingDate.Hour == targetHour)
                .ToList();

            foreach (var reservation in filteredReservations)
            {
                var layout = new LayoutByHour
                {
                    TableId = table.Id,
                    Table = _mapper.Map<LayoutTableDTO>(table),
                    Status = AVAILABLE_STATUS.Reserved
                };
                layouts.Add(layout);
            }
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateChairReservationLayouts(IEnumerable<Chair> chairReservations, DateTime targetDate, int targetHour)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var chair in chairReservations)
        {
            var filteredReservations = chair.ChairReservations
                .Where(x => x.BookingDate.Date == targetDate && x.BookingDate.Hour == targetHour)
                .ToList();

            foreach (var reservation in filteredReservations)
            {
                var layout = new LayoutByHour
                {
                    ChairId = chair.Id,
                    Chair = _mapper.Map<ChairDTO>(chair),
                    Status = AVAILABLE_STATUS.Reserved
                };
                layouts.Add(layout);
            }
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateTableWalkInLayouts(IEnumerable<Table> tableWalkIns, DateTime targetDate, int targetHour)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var table in tableWalkIns)
        {
            var filteredWalkIns = table.WalkIns
                .Where(x => x.WalkInAt.Date == targetDate && x.WalkInAt.Hour == targetHour)
                .ToList();

            foreach (var walkin in filteredWalkIns)
            {
                var layout = new LayoutByHour
                {
                    TableId = table.Id,
                    Table = _mapper.Map<LayoutTableDTO>(table),
                    Status = AVAILABLE_STATUS.WalkIn
                };
                layouts.Add(layout);
            }
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateChairWalkInLayouts(IEnumerable<Chair> chairWalkIns, DateTime targetDate, int targetHour)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var chair in chairWalkIns)
        {
            var filteredWalkIns = chair.WalkIns
                .Where(x => x.WalkInAt.Date == targetDate && x.WalkInAt.Hour == targetHour)
                .ToList();

            foreach (var walkin in filteredWalkIns)
            {
                var layout = new LayoutByHour
                {
                    ChairId = chair.Id,
                    Chair = _mapper.Map<ChairDTO>(chair),
                    Status = AVAILABLE_STATUS.WalkIn
                };
                layouts.Add(layout);
            }
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateEmptyTableLayouts(IEnumerable<Table> noTableBookings, IEnumerable<object> spaceBookings, IEnumerable<object> spaceReservations)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var table in noTableBookings)
        {
            AVAILABLE_STATUS status;
            
            if (spaceBookings.Any())
                status = AVAILABLE_STATUS.Booked;
            else if (spaceReservations.Any())
                status = AVAILABLE_STATUS.Reserved;
            else
                status = AVAILABLE_STATUS.None;

            var layout = new LayoutByHour
            {
                TableId = table.Id,
                Table = _mapper.Map<LayoutTableDTO>(table),
                Status = status
            };
            layouts.Add(layout);
        }
        
        return layouts;
    }

    public List<LayoutByHour> CreateEmptyChairLayouts(IEnumerable<Chair> noChairBookings, IEnumerable<object> spaceBookings, IEnumerable<object> spaceReservations, DateTime targetDate, int targetHour)
    {
        var layouts = new List<LayoutByHour>();
        
        foreach (var chair in noChairBookings)
        {
            AVAILABLE_STATUS status;
            
            if (spaceBookings.Any())
                status = AVAILABLE_STATUS.Booked;
            else if (spaceReservations.Any())
                status = AVAILABLE_STATUS.Reserved;
            else if (chair.Table.Bookings.Any(x => x.BookingDate.Date == targetDate && x.BookingDate.Hour == targetHour))
                status = AVAILABLE_STATUS.Booked;
            else if (chair.Table.TableReservations.Any(x => x.BookingDate.Date == targetDate && x.BookingDate.Hour == targetHour))
                status = AVAILABLE_STATUS.Reserved;
            else if (chair.Table.WalkIns.Any(x => x.WalkInAt.Date == targetDate && x.WalkInAt.Hour == targetHour))
                status = AVAILABLE_STATUS.WalkIn;
            else
                status = AVAILABLE_STATUS.None;

            var layout = new LayoutByHour
            {
                ChairId = chair.Id,
                Chair = _mapper.Map<ChairDTO>(chair),
                Status = status
            };
            layouts.Add(layout);
        }
        
        return layouts;
    }
}