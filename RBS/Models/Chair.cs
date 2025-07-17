namespace RBS.Models;

public class Chair
{
    public int Id { get; set; }
    
    public string ChairNumber { get; set; }
    public decimal? MinSpent { get; set; }
    public bool IsAvailable { get; set; } = true;
    public decimal ChairPrice { get; set; }
    public int Xlocation { get; set; }
    public int Ylocation { get; set; }
    
    public int TableId { get; set; }
    public Table Table { get; set; }
    public List<Booking> Bookings { get; set; } = new List<Booking>();
    public List<ChairReservation> ChairReservations { get; set; } = new List<ChairReservation>();
    public List<WalkIn> WalkIns { get; set; } = new List<WalkIn>();
}