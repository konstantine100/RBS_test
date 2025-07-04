using RBS.Enums;

namespace RBS.Requests;

public class AddFood
{
    public string Name { get; set; }
    public string ImageURL { get; set; }
    public decimal Price { get; set; }
    public FOOD_TYPE FoodType { get; set; }
}