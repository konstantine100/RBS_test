using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.Data;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Services.Implenetation;

public class ConflictTableService : IConflictTableService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public ConflictTableService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<Booking>> ConflictTableBookings(int spaceId, int tableId, DateTime startDate)
    {
        TimeSpan after18Hour = new TimeSpan(18, 0, 0);

        List<Booking> allConflicts = new List<Booking>();
        
        List<Booking> conflictSpaceBookings = await _context.Bookings
            .Include(x => x.Spaces)
            .ThenInclude(x => x.Bookings)
            .Where(x => x.Spaces
                .Any(x => x.Id == spaceId && x.Bookings
                    .Any(x => x.BookingDate.Day == startDate.Day)))
            .ToListAsync();
        
        List<Booking> conflictTableAndChairBookings = await _context.Bookings
            .Include(x => x.Tables)
            .ThenInclude(x => x.Chairs)
            .ThenInclude(x => x.Bookings)
            .Where(x => x.Tables
                            .Any(x => (x.Id == tableId || x.Chairs.Any(y => y.TableId == tableId)))&&
                        x.BookingDate.Day == startDate.Day)
            .ToListAsync();
        
        allConflicts.AddRange(conflictTableAndChairBookings);
        
        if (startDate.Hour < after18Hour.Hours)
        {
            allConflicts = allConflicts
                .Where(x => (x.BookingDate - startDate).Duration() < TimeSpan.FromHours(1) &&
                            (x.BookingDate - startDate).Duration() > TimeSpan.FromHours(-1))
                .ToList();
        }

        else if (startDate.Hour > after18Hour.Hours)
        {
            allConflicts = allConflicts
                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                                    (x.BookingDate <= startDate ||
                                     (x.BookingDate - startDate).Duration() < TimeSpan.FromHours(-1)))
                .ToList();
        }
        conflictSpaceBookings = conflictSpaceBookings
            .Where(x => x.BookingDateEnd > startDate &&
                        (x.BookingDate - startDate).Duration() > TimeSpan.FromHours(-1))
            .ToList();
        
        allConflicts.AddRange(conflictSpaceBookings);
        
        return allConflicts;
    }

    public async Task<List<SpaceReservation>> ConflictSpaceReservation(int spaceId, DateTime startDate)
    {
        List<SpaceReservation> ConflictSpaceReservation = await _context.SpaceReservations
            .Where(x => x.SpaceId == spaceId && 
                        x.BookingDate.Day == startDate.Day)
            .ToListAsync();
        
        ConflictSpaceReservation = ConflictSpaceReservation
            .Where(x => x.BookingDateEnd > startDate &&
                        (x.BookingDate - startDate).Duration() > TimeSpan.FromHours(-1))
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
                .Where(x => (x.BookingDate - startDate).Duration() < TimeSpan.FromHours(1) &&
                            (x.BookingDate - startDate).Duration() > TimeSpan.FromHours(-1))
                .ToList();
        }

        else if (startDate.Hour > after18Hour.Hours)
        {
            conflictTableReservations = conflictTableReservations
                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                            (x.BookingDate <= startDate ||
                             (x.BookingDate - startDate).Duration() < TimeSpan.FromHours(-1)))
                .ToList();
        }
        
        return conflictTableReservations;
    }

    public async Task<List<ChairReservation>> ConflictChairReservation(int tableId, DateTime startDate)
    {
        TimeSpan after18Hour = new TimeSpan(18, 0, 0);

        List<ChairReservation> conflictChairReservations = await _context.ChairReservations
            .Include(x => x.Chair)
            .Where(x => x.Chair.TableId == tableId &&
                        x.BookingDate.Day == startDate.Day)
            .ToListAsync();
        
        if (startDate.Hour < after18Hour.Hours)
        {
            conflictChairReservations = conflictChairReservations
                .Where(x => (x.BookingDate - startDate).Duration() < TimeSpan.FromHours(1) &&
                            (x.BookingDate - startDate).Duration() > TimeSpan.FromHours(-1))
                .ToList();
        }

        else if (startDate.Hour > after18Hour.Hours)
        {
            conflictChairReservations = conflictChairReservations
                .Where(x => x.BookingDate.Hour > after18Hour.Hours &&
                            (x.BookingDate <= startDate ||
                             (x.BookingDate - startDate).Duration() < TimeSpan.FromHours(-1)))
                .ToList();
        }
        
        return conflictChairReservations;
    }
}