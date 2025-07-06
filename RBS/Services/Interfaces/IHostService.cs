using RBS.CORE;
using RBS.DTOs;

namespace RBS.Services.Interfaces;

public interface IHostService
{
    Task<ApiResponse<List<BookingDTO>>> GetRestaurantBookings(int restaurantId);
    Task<ApiResponse<List<LayoutByHour>>> GetCurrentLayout(int spaceId); // websocket
    Task<ApiResponse<BookingDTO>> UpdateBookingLateTimes(int bookingId, TimeSpan lateTime);
    Task<ApiResponse<BookingDTO>> BookingUserAnnounced(int bookingId);
    Task<ApiResponse<BookingDTO>> BookingUserNotAnnounced(int bookingId);
    Task<ApiResponse<BookingDTO>> FinishBooking(int bookingId);
    Task<ApiResponse<TableDTO>> TableAvailabilityChange(int hostId, int tableId);
    
}