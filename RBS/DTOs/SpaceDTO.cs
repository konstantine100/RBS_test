using RBS.Enums;

namespace RBS.DTOs;

public class SpaceDTO
{
    public Guid Id { get; set; }
    public SPACE_TYPE SpaceType { get; set; }
    public decimal SpacePrice { get; set; }
    public bool IsAvailable { get; set; }
    public List<TableDTO> Tables { get; set; }
}