using RBS.Enums;

namespace RBS.Models;

public class OrderedFood
{
    public int Id { get; set; }
    public Food Food { get; set; }
    public int Quantity { get; set; }
    public decimal OverallPrice { get; set; }
    public string? MessageToStuff { get; set; }
    public PAYMENT_STATUS PaymentStatus { get; set; } = PAYMENT_STATUS.NOT_PAYED;
    public int? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public int? WalkInId { get; set; }
    public WalkIn? WalkIn { get; set; }
}