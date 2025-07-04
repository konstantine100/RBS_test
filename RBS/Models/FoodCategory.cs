namespace RBS.Models;

public class FoodCategory
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; }
    List<Food> Foods { get; set; } = new List<Food>();
}