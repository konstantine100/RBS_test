using RBS.Enums;

namespace RBS.Requests;

public class AddFood
{
    public string EnglishName { get; set; }
    public string GeorgianName { get; set; }
    public decimal Price { get; set; }
    public FOOD_TYPE FoodType { get; set; }
}