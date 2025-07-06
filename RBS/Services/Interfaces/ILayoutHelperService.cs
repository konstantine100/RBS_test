using RBS.CORE;
using RBS.Models;

namespace RBS.Services.Interfaces;

public interface ILayoutHelperService
{
    List<Booking> ReturnFilteredSpaceBookings(Space space, DateTime Date);
    List<Table> ReturnFilteredTableBookings(List<Table> tables, DateTime Date);
    List<Chair> ReturnFilteredChairBookings(List<Chair> chairs, DateTime Date);
    List<SpaceReservation> ReturnFilteredSpaceReservations(Space space, DateTime Date);
    List<Table> ReturnFilteredTableReservations(List<Table> tables, DateTime Date);
    List<Chair> ReturnFilteredChairReservations(List<Chair> chairs, DateTime Date);
    List<Table> ReturnFilteredTableWalkIn(List<Table> tables, DateTime Date);
    List<Chair> ReturnFilteredChairWalkIn(List<Chair> chairs, DateTime Date);
    Task<List<Table>> ReturnEmptyTables(int spaceId, DateTime Date);
    Task<List<Chair>> ReturnEmptyChairs(int spaceId, DateTime Date);
    List<LayoutByHour> CreateSpaceBookingLayouts(IEnumerable<object> spaceBookings, int spaceId, Space space);
    List<LayoutByHour> CreateTableBookingLayouts(IEnumerable<Table> tableBookings, DateTime targetDate, int targetHour);
    List<LayoutByHour> CreateChairBookingLayouts(IEnumerable<Chair> chairBookings, DateTime targetDate, int targetHour);
    List<LayoutByHour> CreateSpaceReservationLayouts(IEnumerable<object> spaceReservations, int spaceId, Space space);
    List<LayoutByHour> CreateTableReservationLayouts(IEnumerable<Table> tableReservations, DateTime targetDate, int targetHour);
    List<LayoutByHour> CreateChairReservationLayouts(IEnumerable<Chair> chairReservations, DateTime targetDate, int targetHour);
    List<LayoutByHour> CreateTableWalkInLayouts(IEnumerable<Table> tableWalkIns, DateTime targetDate, int targetHour);
    List<LayoutByHour> CreateChairWalkInLayouts(IEnumerable<Chair> chairWalkIns, DateTime targetDate, int targetHour);
    List<LayoutByHour> CreateEmptyTableLayouts(IEnumerable<Table> noTableBookings, IEnumerable<object> spaceBookings, IEnumerable<object> spaceReservations);
    List<LayoutByHour> CreateEmptyChairLayouts(IEnumerable<Chair> noChairBookings, IEnumerable<object> spaceBookings, IEnumerable<object> spaceReservations, DateTime targetDate, int targetHour);
}