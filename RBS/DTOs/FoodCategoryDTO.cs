namespace RBS.DTOs;

public class FoodCategoryDTO
{
    public string CategoryName { get; set; }
    List<FoodDTO> Foods { get; set; }
}