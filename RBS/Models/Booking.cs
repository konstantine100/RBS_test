namespace RBS.Models;

public class Booking
{
    public Guid Id { get; set; }

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime BookingDate { get; set; } 
    public DateTime? BookingDateEnd { get; set; } // marto roca mtlian sivrces qiraobs!!!
    public DateTime BookingExpireDate { get; set; }
    
    public bool IsPayed { get; set; } = false;
    public bool IsFinished { get; set; } = false;
    public decimal Price { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid? TableId { get; set; }
    public Table? Table { get; set; }
    public List<Chair> Chairs { get; set; } = new List<Chair>();
    public Guid? SpaceId { get; set; }
    public Space? Space { get; set; }
    
}