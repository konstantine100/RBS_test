using RBS.CORE;
using RBS.DTOs;

namespace RBS.Services.Interfaces;

public interface IMenuService
{
    Task<ApiResponse<MenuDTO>> AddMenu(int restaurantId);
    Task<ApiResponse<MenuDTO>> DeleteMenu(int menuId);
    Task<ApiResponse<MenuDTO>> SeeMenu(int restaurantId);
    Task<ApiResponse<List<FoodDTO>>> SearchFoodInMenu(int menuId, string searchTerm);
}