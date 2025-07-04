namespace RBS.Models;

public class Menu
{
    public int Id { get; set; }
    public List<FoodCategory> Categories { get; set; } =  new List<FoodCategory>();
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
}