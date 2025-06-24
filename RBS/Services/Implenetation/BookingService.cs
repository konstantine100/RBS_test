using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;
using RBS.Services.Interfaces;

namespace RBS.Services.Implenetation;

public class BookingService : IBookingService
{
    public ApiResponse<RestaurantDTO> AddRestaurant(AddRestaurant request)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<SpaceDTO> AddSpace(Guid restaurantId, AddSpace request)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<TableDTO> AddTable(Guid spaceId, AddTable request)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<ChairDTO> AddChair(Guid tableId, AddChair request)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<RestaurantDTO> DeleteRestaurant(Guid restaurantId)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<SpaceDTO> DeleteSpace(Guid spaceId)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<TableDTO> DeleteTable(Guid tableId)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<ChairDTO> DeleteChair(Guid chairId)
    {
        throw new NotImplementedException();
    }
}