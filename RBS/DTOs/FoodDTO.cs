using RBS.Enums;

namespace RBS.DTOs;

public class FoodDTO
{
    public string EnglishName { get; set; }
    public string GeorgianName { get; set; }
    public decimal Price { get; set; }
    public FOOD_TYPE FoodType { get; set; }
    public bool IsAvailable { get; set; }
    public List<IngredientDTO> Ingredients { get; set; }
}