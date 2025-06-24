using RBS.Enums;

namespace RBS.Requests;

public class AddSpace
{
    public SPACE_TYPE SpaceType { get; set; }
    public decimal SpacePrice { get; set; }
}