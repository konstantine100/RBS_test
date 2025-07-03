using RBS.Enums;

namespace RBS.Models;

public class Table
{
    public int Id { get; set; }
    
    public TABLE_TYPE TableType { get; set; }
    public TABLE_SHAPE TableShape { get; set; }
    public int ChairQuantity { get; set; } = 0; // გავწეროთ კონსტრუქტორში
    public string TableNumber { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsFilled { get; set; } = false;
    public decimal TablePrice { get; set; }
    public decimal? MinSpent { get; set; }
    public int Xlocation { get; set; }
    public int Ylocation { get; set; }
    
    public int SpaceId { get; set; }
    public Space Space { get; set; }
    public List<Chair> Chairs { get; set; } = new List<Chair>();
    public List<Booking> Bookings { get; set; } = new List<Booking>();
    public List<TableReservation> TableReservations { get; set; } = new List<TableReservation>();

    public Table()
    {
        ChairQuantity = Chairs.Count;
    }
}