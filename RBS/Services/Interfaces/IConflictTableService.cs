using RBS.Models;

namespace RBS.Services.Interfaces;

public interface IConflictTableService
{
    Task<List<Booking>> ConflictTableBookings(int spaceId, int tableId, DateTime startDate);
    Task<List<SpaceReservation>> ConflictSpaceReservation(int spaceId, DateTime startDate);
    Task<List<TableReservation>> ConflictTableReservation(int tableId, DateTime startDate);
    Task<List<ChairReservation>> ConflictChairReservation(int tableId, DateTime startDate);
    
}