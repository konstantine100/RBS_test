using RBS.Enums;

namespace RBS.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public FOOD_TYPE FoodType { get; set; }
    public int FoodId { get; set; }
    public Food Food { get; set; }
}