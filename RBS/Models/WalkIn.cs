namespace RBS.Models;

public class WalkIn
{
    public int Id { get; set; }
    public DateTime WalkInAt { get; set; } = DateTime.UtcNow;
    public int? TableId { get; set; }
    public Table? Table { get; set; }
    public int? ChairId { get; set; }
    public Chair? Chair { get; set; }
    public int spaceId { get; set; }
    public bool IsFinished { get; set; } = false;
    
    public int HostId { get; set; }
    public User Host { get; set; }
}