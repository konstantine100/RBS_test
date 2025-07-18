using RBS.Models;

namespace RBS.Services.Interfaces;

public interface IConflictTableService
{
    Task<List<Booking>> ConflictTableBookings(int spaceId, int tableId, DateTime startDate, DateTime endDate);
    Task<List<SpaceReservation>> ConflictSpaceReservation(int spaceId, DateTime startDate, DateTime endDate);
    Task<List<TableReservation>> ConflictTableReservation(int tableId, DateTime startDate, DateTime endDate);
    Task<List<ChairReservation>> ConflictChairReservation(int tableId, DateTime startDate, DateTime endDate);
    Task<List<WalkIn>> ConflictWalkIn(int tableId, DateTime startDate);
    
}