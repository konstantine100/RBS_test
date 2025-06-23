using RBS.Enums;

namespace RBS.Models;

public class Space
{
    public Guid Id { get; set; }
    public SPACE_TYPE SpaceType { get; set; } // enum ? 
    
    public Guid RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
    public decimal SpacePrice { get; set; }
    
    public List<Table> Tables { get; set; } = new List<Table>();

    // space -> tables list
    // tables -> chairs list
}