using RBS.Enums;

namespace RBS.Requests;

public class AddIngredient
{
    public string Name { get; set; }
    public FOOD_TYPE FoodType { get; set; }
}