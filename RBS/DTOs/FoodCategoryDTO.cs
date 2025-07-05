namespace RBS.DTOs;

public class FoodCategoryDTO
{
    public string CategoryEnglishName { get; set; }
    public string CategoryGeorgianName { get; set; }
    public List<FoodDTO> Foods { get; set; }
}