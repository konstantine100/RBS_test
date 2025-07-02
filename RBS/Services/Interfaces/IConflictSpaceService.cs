using RBS.Models;

namespace RBS.Services.Interfaces;

public interface IConflictSpaceService
{
    Task<List<Booking>> ConflictSpaceBookings(int spaceId, DateTime startDate, DateTime endDate);
    Task<List<SpaceReservation>> ConflictSpaceReservation(int spaceId, DateTime startDate, DateTime endDate);
    Task<List<TableReservation>> ConflictSpaceTableReservation(int spaceId, DateTime startDate, DateTime endDate);
    Task<List<ChairReservation>> ConflictSpaceChairReservation(int spaceId, DateTime startDate, DateTime endDate);
}