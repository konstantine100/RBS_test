using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IFoodCategoryService
{
    Task<ApiResponse<FoodCategoryDTO>> AddFoodCategory(int menuId, AddFoodCategory request);
    Task<ApiResponse<FoodCategoryDTO>> UpdateFoodCategory(int categoryId, bool IsEnglish ,string newCategoryName);
    Task<ApiResponse<FoodCategoryDTO>> SeeFoodCategory(int categoryId);
    Task<ApiResponse<FoodCategoryDTO>> DeleteFoodCategory(int categoryId);

}