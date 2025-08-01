namespace RBS.DTOs;

public class EventDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime EventDate { get; set; }
}