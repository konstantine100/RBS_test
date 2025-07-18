using RBS.Enums;

namespace RBS.Models;

public class FoodCategory
{
    public int Id { get; set; }
    public string CategoryEnglishName { get; set; }
    public string CategoryGeorgianName { get; set; }
    public FOOD_CATEGORY_TYPE FoodCategoryType { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; }
    public List<Food> Foods { get; set; } = new List<Food>();
}