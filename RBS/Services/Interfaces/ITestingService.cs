using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface ITestingService
{
    ApiResponse<RestaurantDTO> AddRestaurant(AddRestaurant request);
    ApiResponse<SpaceDTO> AddSpace(int restaurantId, AddSpace request);
    ApiResponse<TableDTO> AddTable(int spaceId, AddTable request);
    ApiResponse<ChairDTO> AddChair(int tableId, AddChair request);
    
    // delete
    ApiResponse<RestaurantDTO> DeleteRestaurant(int restaurantId);
    ApiResponse<SpaceDTO> DeleteSpace(int spaceId);
    ApiResponse<TableDTO> DeleteTable(int tableId);
    ApiResponse<ChairDTO> DeleteChair(int chairId);
}