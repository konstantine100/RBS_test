using RBS.Models;

namespace RBS.Services.Interfaces;

public interface IConflictChairService
{
    Task<List<Booking>> ConflictChairBookings(int chairId, DateTime startDate, DateTime endDate);
    Task<List<SpaceReservation>> ConflictSpaceReservation(int spaceId, DateTime startDate, DateTime endDate);
    Task<List<TableReservation>> ConflictTableReservation(int tableId, DateTime startDate, DateTime endDate);
    Task<List<ChairReservation>> ConflictChairReservation(int chairId, DateTime startDate, DateTime endDate);
    Task<List<WalkIn>> ConflictWalkIn(int chairId, DateTime startDate);
}