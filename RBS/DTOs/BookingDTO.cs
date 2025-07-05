using RBS.Enums;

namespace RBS.DTOs;

public class BookingDTO
{
    public int Id { get; set; }
    public DateTime BookedAt { get; set; } 
    public DateTime BookingDate { get; set; } 
    public DateTime? BookingDateEnd { get; set; }
    public DateTime BookingExpireDate { get; set; }
    public decimal Price { get; set; }
    public PAYMENT_STATUS PaymentStatus { get; set; }
    public BOOKING_STATUS BookingStatus { get; set; } 
    public UserDTO User { get; set; }
    public List<SpaceDTO> Spaces { get; set; }
    public List<TableDTO> Tables { get; set; }
    public List<ChairDTO> Chairs { get; set; }
}