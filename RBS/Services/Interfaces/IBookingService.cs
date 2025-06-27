using RBS.CORE;
using RBS.DTOs;
using RBS.Helpers;
using RBS.Models;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IBookingService
{
    ApiResponse<List<LayoutByHour>> GetReservationsByHour (Guid spaceId, DateTime Date);
    ApiResponse<BookingDTO> ChooseSpace(Guid userId ,Guid spaceId, AddBooking request, DateTime endDate);
    ApiResponse<BookingDTO> ChooseTable(Guid userId ,Guid tableId, AddBooking request);
    ApiResponse<BookingDTO> ChooseChair(Guid userId ,Guid chairId, AddBooking request);
    ApiResponse<BookingDTO> ChooseAnotherSpace(Guid userId, Guid bookingId, Guid spaceId);
    ApiResponse<BookingDTO> ChooseAnotherTable(Guid userId, Guid bookingId, Guid tableId);
    ApiResponse<BookingDTO> ChooseAnotherChair(Guid userId, Guid bookingId, Guid chairId);
    ApiResponse<BookingDTO> CompleteBooking(Guid userId, Guid bookingId);
    ApiResponse<BookingDTO> RemoveBookingSpace(Guid userId, Guid bookingId, Guid spaceId);
    ApiResponse<BookingDTO> RemoveBookingTable(Guid userId, Guid bookingId, Guid tableId);
    ApiResponse<BookingDTO> RemoveBookingChair(Guid userId, Guid bookingId, Guid chairId);
    ApiResponse<BookingDTO> RemoveBooking(Guid userId, Guid bookingId);
}