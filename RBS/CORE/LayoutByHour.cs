using RBS.DTOs;
using RBS.Enums;
using RBS.Models;

namespace RBS.CORE;

public class LayoutByHour
{
    public int? SpaceId { get; set; }
    public SpaceDTO? Space { get; set; }
    public int? TableId { get; set; }
    public TableDTO? Table { get; set; }
    public int? ChairId { get; set; }
    public ChairDTO? Chair { get; set; }
    public AVAILABLE_STATUS Status { get; set; }
}