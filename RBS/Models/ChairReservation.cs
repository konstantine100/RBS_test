using RBS.Enums;

namespace RBS.Models;

public class ChairReservation
{
    public int Id { get; set; }

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime BookingDate { get; set; } 
    public DateTime BookingExpireDate { get; set; }
    
    public decimal Price { get; set; }
    public PAYMENT_STATUS PaymentStatus { get; set; } = PAYMENT_STATUS.NOT_PAYED;
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int ChairId { get; set; }
    public Chair Chair { get; set; }
}