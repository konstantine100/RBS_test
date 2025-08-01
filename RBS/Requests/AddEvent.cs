namespace RBS.Requests;

public class AddEvent
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime EventDate { get; set; }
}