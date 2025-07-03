using RBS.DTOs;

namespace RBS.CORE;

public class OverallReservations
{
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime BookingDate { get; set; } 
    public DateTime? BookingDateEnd { get; set; } // marto roca mtlian sivrces qiraobs!!!
    public DateTime BookingExpireDate { get; set; }
    
    public decimal Price { get; set; }
    public SpaceDTO? Space { get; set; }
    public TableDTO? Table { get; set; }
    public ChairDTO? Chair { get; set; }
}