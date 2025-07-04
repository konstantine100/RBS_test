using RBS.Enums;

namespace RBS.DTOs;

public class FoodDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public FOOD_TYPE FoodType { get; set; }
    public bool IsAvailable { get; set; }
    List<IngredientDTO> Ingredients { get; set; }
}