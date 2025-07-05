namespace RBS.DTOs;

public class WalkInDTO
{
    public int Id { get; set; }
    public DateTime WalkInAt { get; set; }
    public TableDTO? Table { get; set; }
    public ChairDTO? Chair { get; set; }
    public bool IsFinished { get; set; }
}