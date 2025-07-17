namespace RBS.DTOs;

public class SpaceReservationDTO
{
    public int Id { get; set; }

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime BookingDate { get; set; } 
    public DateTime? BookingDateEnd { get; set; } // marto roca mtlian sivrces qiraobs!!!
    public DateTime BookingExpireDate { get; set; }
    
    public decimal Price { get; set; }
    
    public UserDTO User { get; set; }
    public SpaceDTO Space { get; set; }
}