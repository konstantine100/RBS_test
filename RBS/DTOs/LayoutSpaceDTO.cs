using RBS.Enums;

namespace RBS.DTOs;

public class LayoutSpaceDTO
{
    public int Id { get; set; }
    public SPACE_TYPE SpaceType { get; set; }
    public decimal SpacePrice { get; set; }
    public bool IsAvailable { get; set; }
}