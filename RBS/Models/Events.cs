namespace RBS.Models;

public class Events
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    public DateTime EventDate { get; set; }

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
}