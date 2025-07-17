using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IWalkInOrderFoodService
{
    Task<ApiResponse<OrderedFoodDTO>> WalkInOrderFood(int hostId, int walkInId, int foodId, AddOrderedFood request);
    Task<ApiResponse<List<OrderedFoodDTO>>> GetWalkInOrderedFoods(int hostId, int walkInId);
    Task<ApiResponse<List<OrderedFoodDTO>>> GetRestaurantOrderedFoods(int restaurantId);
    Task<ApiResponse<OrderedFoodDTO>> ChangeWalkInOrderFoodQuantity(int hostId, int orderedFoodId, int quantity);
    Task<ApiResponse<OrderedFoodDTO>> ChangeWalkInOrderFoodMessage(int hostId, int orderedFoodId, string? message);
    Task<ApiResponse<List<OrderedFoodDTO>>> ChangeOrderFoodPaymentStatus(int hostId, int walkInId);
    Task<ApiResponse<OrderedFoodDTO>> DeleteOrderFood(int hostId, int orderedFoodId);
}