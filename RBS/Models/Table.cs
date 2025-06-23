using RBS.Enums;

namespace RBS.Models;

public class Table
{
    public Guid Id { get; set; }
    
    public TABLE_TYPE TableType { get; set; }
    public TABLE_SHAPE TableShape { get; set; }
    public int ChairQuantity { get; set; } = 0; // გავწეროთ კონსტრუქტორში
    public string TableNumber { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsFilled { get; set; } = false;
    public decimal TablePrice { get; set; }

    public decimal? MinSpent { get; set; }
    
    public Guid SpaceId { get; set; }
    public Space Space { get; set; }
    public List<Chair> Chairs { get; set; } = new List<Chair>();


    public Table()
    {
        ChairQuantity = Chairs.Count;
    }
}