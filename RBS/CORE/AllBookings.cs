using RBS.DTOs;

namespace RBS.CORE;

public class AllBookings
{
    public List<BookingDTO> Bookings { get; set; } = new List<BookingDTO>();
}