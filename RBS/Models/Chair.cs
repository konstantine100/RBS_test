namespace RBS.Models;

public class Chair
{
    public Guid Id { get; set; }
    
    public string ChairNumber { get; set; }
    public decimal? MinSpent { get; set; }
    public bool IsBooked { get; set; } = false;
    public bool IsAvailable { get; set; } = true;
    public decimal ChairPrice { get; set; }
    
    public Guid TableId { get; set; }
    public Table Table { get; set; }
}