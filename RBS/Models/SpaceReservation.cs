namespace RBS.Models;

public class SpaceReservation
{
    public int Id { get; set; }

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime BookingDate { get; set; } 
    public DateTime? BookingDateEnd { get; set; } // marto roca mtlian sivrces qiraobs!!!
    public DateTime BookingExpireDate { get; set; }
    
    public decimal Price { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int SpaceId { get; set; }
    public Space Space { get; set; }
}