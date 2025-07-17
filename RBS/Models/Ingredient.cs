using RBS.Enums;

namespace RBS.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string EnglishName { get; set; }
    public string GeorgianName { get; set; }
    public int FoodId { get; set; }
    public Food Food { get; set; }
}