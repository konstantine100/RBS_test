using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.Data;
using RBS.Enums;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Services.Implenetation;

public class ConflictChairService : IConflictChairService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public ConflictChairService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<Booking>> ConflictChairBookings(int chairId, DateTime startDate)
    {
        TimeSpan after18Hour = new TimeSpan(18, 0, 0);

        List<Booking> allBookings = new List<Booking>();
        
        List<Booking> chairBookings = await _context.Bookings
            .Include(x => x.Chairs)
            .Where(x => (x.BookingStatus == BOOKING_STATUS.Waiting || 
                         x.BookingStatus == BOOKING_STATUS.Announced) &&
                        x.Chairs
                            .Any(x => x.Id == chairId))
            .ToListAsync();
        
        List<Booking> tableConflicts = await _context.Bookings
            .Include(x => x.Tables)
            .ThenInclude(x => x.Chairs)
            .Where(x => (x.BookingStatus == BOOKING_STATUS.Waiting || 
                         x.BookingStatus == BOOKING_STATUS.Announced) && x.Tables
                            .Any(x => x.Chairs
                                .Any(x => x.Id == chairId)) &&
                        x.BookingDate.Day == startDate.Day)
            .ToListAsync();
        
        List<Booking> spaceConflicts = await _context.Bookings
            .Include(x => x.Spaces)
            .ThenInclude(x => x.Tables)
            .ThenInclude(x => x.Chairs)
            .Where(x => (x.BookingStatus == BOOKING_STATUS.Waiting || 
                         x.BookingStatus == BOOKING_STATUS.Announced) && x.Spaces
                            .Any(x => x.Tables
                                .Any(x => x.Chairs
                                    .Any(x => x.Id == chairId))) &&
                        x.BookingDate.Day == startDate.Day)
            .ToListAsync();
        
        allBookings.AddRange(chairBookings);
        allBookings.AddRange(tableConflicts);
        
        if (startDate.Hour < after18Hour.Hours)
        {
            allBookings = allBookings
                .Where(x => (x.BookingDate - startDate).TotalHours < 1 &&
                            (x.BookingDate - startDate).TotalHours > -1)
                .ToList();
        }
        else if (startDate.Hour > after18Hour.Hours)
        {
            allBookings = allBookings
                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                            (x.BookingDate <= startDate ||
                             (x.BookingDate - startDate).TotalHours < 1))
                .ToList();
        }
        spaceConflicts = spaceConflicts
            .Where(x => x.BookingDateEnd > startDate &&
                        (x.BookingDate - startDate).TotalHours > -1)
            .ToList();
        
        allBookings.AddRange(spaceConflicts);
        
        return allBookings;
    }

    public async Task<List<SpaceReservation>> ConflictSpaceReservation(int spaceId, DateTime startDate)
    {
        List<SpaceReservation> ConflictSpaceReservation = await _context.SpaceReservations
            .Where(x => x.SpaceId == spaceId && 
                        x.BookingDate.Day == startDate.Day)
            .ToListAsync();
        
        ConflictSpaceReservation = ConflictSpaceReservation
            .Where(x => x.BookingDateEnd > startDate &&
                        (x.BookingDate - startDate).TotalHours > -1)
            .ToList();
        
        return ConflictSpaceReservation;
    }

    public async Task<List<TableReservation>> ConflictTableReservation(int tableId, DateTime startDate)
    {
        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
        
        List<TableReservation> conflictTableReservations = await _context.TableReservations
            .Where(x => x.TableId == tableId && 
                        x.BookingDate.Day == startDate.Day)
            .ToListAsync();
        
        if (startDate.Hour < after18Hour.Hours)
        {
            conflictTableReservations = conflictTableReservations
                .Where(x => (x.BookingDate - startDate).TotalHours < 1 &&
                            (x.BookingDate - startDate).TotalHours > -1)
                .ToList();
        }

        else if (startDate.Hour > after18Hour.Hours)
        {
            conflictTableReservations = conflictTableReservations
                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                            (x.BookingDate <= startDate ||
                             (x.BookingDate - startDate).TotalHours < 1))
                .ToList();
        }
        
        return conflictTableReservations;
    }

    public async Task<List<ChairReservation>> ConflictChairReservation(int chairId, DateTime startDate)
    {
        TimeSpan after18Hour = new TimeSpan(18, 0, 0);

        List<ChairReservation> conflictChairReservations = await _context.ChairReservations
            .Where(x => x.ChairId == chairId &&
                        x.BookingDate.Day == startDate.Day)
            .ToListAsync();
        
        if (startDate.Hour < after18Hour.Hours)
        {
            conflictChairReservations = conflictChairReservations
                .Where(x => (x.BookingDate - startDate).TotalHours < 1 &&
                            (x.BookingDate - startDate).TotalHours > -1)
                .ToList();
        }

        else if (startDate.Hour > after18Hour.Hours)
        {
            conflictChairReservations = conflictChairReservations
                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                            (x.BookingDate <= startDate ||
                             (x.BookingDate - startDate).TotalHours < 1))
                .ToList();
        }
        
        return conflictChairReservations;
    }

    public async Task<List<WalkIn>> ConflictWalkIn(int chairId, DateTime startDate)
    {
        TimeSpan after18Hour = new TimeSpan(18, 0, 0);
        
        List<WalkIn> conflictWalkIns = await _context.WalkIns
            .Include(x => x.Table)
            .ThenInclude(x => x.Chairs)
            .Where(x => x.IsFinished == false &&
                (x.ChairId.HasValue ^ x.TableId.HasValue) &&
                ((x.ChairId.HasValue && x.ChairId == chairId) ||
                 (x.TableId.HasValue && x.Table.Chairs.Any(x => x.Id == chairId))) &&
                x.WalkInAt.Day == startDate.Day)
            .ToListAsync();
        
        if (startDate.Hour < after18Hour.Hours)
        {
            conflictWalkIns = conflictWalkIns
                .Where(x => (x.WalkInAt - startDate).TotalHours < 1 &&
                            (x.WalkInAt - startDate).TotalHours > -1)
                .ToList();
        }

        else if (startDate.Hour > after18Hour.Hours)
        {
            conflictWalkIns = conflictWalkIns
                .Where(x => x.WalkInAt.Hour > after18Hour.Hours &&
                            (x.WalkInAt <= startDate ||
                             (x.WalkInAt - startDate).TotalHours < 1))
                .ToList();
        }
        
        return conflictWalkIns;
    }
}