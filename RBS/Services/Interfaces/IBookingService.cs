using RBS.CORE;
using RBS.DTOs;
using RBS.Helpers;
using RBS.Models;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<AllBookings>> CompleteBooking(int userId);
    Task<ApiResponse<List<BookingDTO>>> MyBookings(int userId);
    Task<ApiResponse<BookingDTO>> GetMyBookingById(int userId, int bookingId);
    Task<ApiResponse<BookingDTO>> ClosestBookingReminder(int userId);
    Task<ApiResponse<List<BookingDTO>>> MyCurrentBookings(int userId);
    Task<ApiResponse<List<BookingDTO>>> MyOldBookings(int userId);
    Task<ApiResponse<BookingDTO>> CancelBooking(int userId, int bookingId);
}