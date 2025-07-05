using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RBS.Data;
using RBS.Models;
using RBS.Services.Interfaces;

namespace RBS.Services.Implenetation;

public class ConflictSpaceService : IConflictSpaceService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    
    public ConflictSpaceService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<Booking>> ConflictSpaceBookings(int spaceId, DateTime startDate, DateTime endDate)
    {
        List<Booking> allConflictBookings = new List<Booking>();
        
        List<Booking> conflictSpaces = await _context.Bookings
            .Include(x => x.Spaces)
            .Where(x => (x.Spaces.Any(x => x.Id == spaceId))  &&
                        (x.BookingDate >= startDate && x.BookingDate <= endDate) )
            .ToListAsync();
                        
        List<Booking> conflictTables = await _context.Bookings
            .Include(x => x.Tables)
            .Where(x => (x.Tables.Any(x => x.SpaceId == spaceId))  &&
                        (x.BookingDate >= startDate && x.BookingDate <= endDate) )
            .ToListAsync();
                        
        List<Booking> conflictChairs = await _context.Bookings
            .Include(x => x.Tables)
            .Where(x => (x.Tables.Any(x => x.SpaceId == spaceId))  &&
                        (x.BookingDate >= startDate && x.BookingDate <= endDate) )
            .ToListAsync();
        
        allConflictBookings.AddRange(conflictSpaces);
        allConflictBookings.AddRange(conflictTables);
        allConflictBookings.AddRange(conflictChairs);
        
        return allConflictBookings;
    }
    
    public async Task<List<SpaceReservation>> ConflictSpaceReservation(int spaceId, DateTime startDate, DateTime endDate)
    {
        List<SpaceReservation> conflictSpaces = await _context.SpaceReservations
            .Include(x => x.Space)
            .Where(x => x.Space.Id == spaceId  &&
                        (x.BookingDate >= startDate && x.BookingDate <= endDate) )
            .ToListAsync();
        
        return conflictSpaces;
    }
    
    public async Task<List<TableReservation>> ConflictSpaceTableReservation(int spaceId, DateTime startDate, DateTime endDate)
    {
        List<TableReservation> conflictTables = await _context.TableReservations
            .Include(x => x.Table)
            .Where(x => x.Table.SpaceId == spaceId  &&
                        (x.BookingDate >= startDate && x.BookingDate <= endDate) )
            .ToListAsync();
        
        return conflictTables;
    }

    public async Task<List<ChairReservation>> ConflictSpaceChairReservation(int spaceId, DateTime startDate, DateTime endDate)
    {
        List<ChairReservation> conflictChairs = await _context.ChairReservations
            .Include(x => x.Chair)
            .ThenInclude(x => x.Table)
            .Where(x => x.Chair.Table.SpaceId == spaceId  &&
                        (x.BookingDate >= startDate && x.BookingDate <= endDate) )
            .ToListAsync();
        
        
        return conflictChairs;
    }

    public async Task<List<WalkIn>> ConflictWalkIns(int spaceId, DateTime startDate, DateTime endDate)
    {
        List<WalkIn> conflictWalkIns = await _context.WalkIns
            .Where(x => x.spaceId == spaceId &&
                        (x.WalkInAt >= startDate && x.WalkInAt <= endDate) )
            .ToListAsync();
        
        return conflictWalkIns;
    }
}