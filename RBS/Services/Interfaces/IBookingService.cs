using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IBookingService
{
    // droebit restoranis/sivrcis/magidis/skamis damatebis funqcionali
    // post 
    ApiResponse<RestaurantDTO> AddRestaurant(AddRestaurant request);
    ApiResponse<SpaceDTO> AddSpace(Guid restaurantId, AddSpace request);
    ApiResponse<TableDTO> AddTable(Guid spaceId, AddTable request);
    ApiResponse<ChairDTO> AddChair(Guid tableId, AddChair request);
    
    // delete
    ApiResponse<RestaurantDTO> DeleteRestaurant(Guid restaurantId);
    ApiResponse<SpaceDTO> DeleteSpace(Guid spaceId);
    ApiResponse<TableDTO> DeleteTable(Guid tableId);
    ApiResponse<ChairDTO> DeleteChair(Guid chairId);
    
    // main part
    ApiResponse<BookingDTO> ChooseSpace(Guid userId ,Guid spaceId, AddBooking request, DateTime endDate);
    ApiResponse<BookingDTO> BookSpace(Guid userId ,Guid bookingId);
    ApiResponse<BookingDTO> ChooseTable(Guid userId ,Guid tableId, AddBooking request);
    ApiResponse<BookingDTO> BookTable(Guid userId ,Guid bookingId);
    ApiResponse<BookingDTO> ChooseChair(Guid userId ,Guid chairId, AddBooking request);
    ApiResponse<BookingDTO> BookChair(Guid userId ,Guid bookingId);
}