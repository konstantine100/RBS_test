using RBS.DTOs;
using RBS.Enums;
using RBS.Models;

namespace RBS.CORE;

public class LayoutByHour
{
    public Guid? SpaceId { get; set; }
    public SpaceDTO? Space { get; set; }
    public Guid? TableId { get; set; }
    public TableDTO? Table { get; set; }
    public Guid? ChairId { get; set; }
    public ChairDTO? Chair { get; set; }
    public AVAILABLE_STATUS Status { get; set; }
}