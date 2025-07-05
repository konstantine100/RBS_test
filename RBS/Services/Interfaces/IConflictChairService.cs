using RBS.Models;

namespace RBS.Services.Interfaces;

public interface IConflictChairService
{
    Task<List<Booking>> ConflictChairBookings(int chairId, DateTime startDate);
    Task<List<SpaceReservation>> ConflictSpaceReservation(int spaceId, DateTime startDate);
    Task<List<TableReservation>> ConflictTableReservation(int tableId, DateTime startDate);
    Task<List<ChairReservation>> ConflictChairReservation(int chairId, DateTime startDate);
    Task<List<WalkIn>> ConflictWalkIn(int chairId, DateTime startDate);
}