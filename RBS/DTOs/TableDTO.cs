using RBS.Enums;

namespace RBS.DTOs;

public class TableDTO
{
    public Guid Id { get; set; }
    public TABLE_TYPE TableType { get; set; }
    public TABLE_SHAPE TableShape { get; set; }
    public int ChairQuantity { get; set; } 
    public string TableNumber { get; set; }
    public bool IsAvailable { get; set; } 
    public bool IsFilled { get; set; } 
    public decimal TablePrice { get; set; }
    public decimal? MinSpent { get; set; }
    public int Xlocation { get; set; }
    public int Ylocation { get; set; }
    public List<ChairDTO> Chairs { get; set; }
}