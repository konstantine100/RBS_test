namespace RBS.Models;

public class ReservationBooking
{
    //not using right now, need to improve
    public int Id { get; set; }

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public DateTime BookingDate { get; set; } 
    public DateTime? BookingDateEnd { get; set; } // marto roca mtlian sivrces qiraobs!!!
    public DateTime BookingExpireDate { get; set; }
    
    public decimal Price { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }

    public List<Table> Tables { get; set; } = new List<Table>();
    public List<Chair> Chairs { get; set; } = new List<Chair>();

    public List<Space> Spaces { get; set; } = new List<Space>();
}