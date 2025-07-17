using RBS.Enums;

namespace RBS.Models;

public class Food
{
    public int Id { get; set; }
    public string EnglishName { get; set; }
    public string GeorgianName { get; set; }
    public decimal Price { get; set; }
    public FOOD_TYPE FoodType { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int FoodCategoryId { get; set; }
    public FoodCategory FoodCategory { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
}