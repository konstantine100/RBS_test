using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IOrderFoodService
{
    Task<ApiResponse<OrderedFoodDTO>> OrderFood(int userId, int bookingId, int foodId, AddOrderedFood request);
    Task<ApiResponse<List<OrderedFoodDTO>>> GetMyOrderedFoods(int userId, int bookingId);
    Task<ApiResponse<List<OrderedFoodDTO>>> GetRestaurantOrderedFoods(int restaurantId);
    Task<ApiResponse<OrderedFoodDTO>> ChangeOrderFoodQuantity(int userId, int orderedFoodId, int quantity);
    Task<ApiResponse<OrderedFoodDTO>> ChangeOrderFoodMessage(int userId, int orderedFoodId, string? message);
    Task<ApiResponse<List<OrderedFoodDTO>>> PayForOrder(int userId, int bookingId);
    Task<ApiResponse<OrderedFoodDTO>> DeleteOrderFood(int userId, int orderedFoodId);
    
}