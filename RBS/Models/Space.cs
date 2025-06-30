using RBS.Enums;

namespace RBS.Models;

public class Space
{
    public Guid Id { get; set; }
    public SPACE_TYPE SpaceType { get; set; } // enum ? 
    public decimal SpacePrice { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    public Guid RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
    
    public List<Table> Tables { get; set; } = new List<Table>();
    public List<Booking> Bookings { get; set; } = new List<Booking>();
    public List<ReservationBooking> BookingReservations { get; set; } = new List<ReservationBooking>();

    // space -> tables list
    // tables -> chairs list
}