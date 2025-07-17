using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IFoodService
{
    Task<ApiResponse<FoodDTO>> AddFood(int categoryId, AddFood request);
    Task<ApiResponse<FoodDTO>> UpdateFood(int foodId, string changeParameter, string changeTo);
    Task<ApiResponse<FoodDTO>> SeeFoodDetails(int foodId);
    Task<ApiResponse<FoodDTO>> DeleteFood(int foodId);
    Task<ApiResponse<FoodDTO>> FoodAvailabilityChange(int foodId);
}