namespace RBS.DTOs;

public class ChairDTO
{
    public Guid Id { get; set; }
    
    public string ChairNumber { get; set; }
    public decimal? MinSpent { get; set; }
    public bool IsBooked { get; set; } 
    public bool IsAvailable { get; set; }
    public decimal ChairPrice { get; set; }
}