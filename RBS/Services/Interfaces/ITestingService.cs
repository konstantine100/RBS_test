using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface ITestingService
{
    ApiResponse<RestaurantDTO> AddRestaurant(AddRestaurant request);
    ApiResponse<SpaceDTO> AddSpace(Guid restaurantId, AddSpace request);
    ApiResponse<TableDTO> AddTable(Guid spaceId, AddTable request);
    ApiResponse<ChairDTO> AddChair(Guid tableId, AddChair request);
    
    // delete
    ApiResponse<RestaurantDTO> DeleteRestaurant(Guid restaurantId);
    ApiResponse<SpaceDTO> DeleteSpace(Guid spaceId);
    ApiResponse<TableDTO> DeleteTable(Guid tableId);
    ApiResponse<ChairDTO> DeleteChair(Guid chairId);
}