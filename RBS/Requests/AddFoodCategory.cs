using RBS.Enums;

namespace RBS.Requests;

public class AddFoodCategory
{
    public string CategoryEnglishName { get; set; }
    public string CategoryGeorgianName { get; set; }
    public FOOD_CATEGORY_TYPE FoodCategoryType { get; set; }
}