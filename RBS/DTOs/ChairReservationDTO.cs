namespace RBS.DTOs;

public class ChairReservationDTO
{
    public int Id { get; set; }

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime BookingDate { get; set; } 
    public DateTime BookingExpireDate { get; set; }
    
    public decimal Price { get; set; }
    
    public UserDTO User { get; set; }
    
    public ChairDTO Chair { get; set; }
}