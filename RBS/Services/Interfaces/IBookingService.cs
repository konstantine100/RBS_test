using RBS.CORE;
using RBS.DTOs;
using RBS.Models;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IBookingService
{
    // main part
    ApiResponse<BookingDTO> ChooseSpace(Guid userId ,Guid spaceId, AddBooking request, DateTime endDate);
    ApiResponse<BookingDTO> ChooseTable(Guid userId ,Guid tableId, AddBooking request);
    ApiResponse<BookingDTO> ChooseChair(Guid userId ,Guid chairId, AddBooking request);
    ApiResponse<BookingDTO> CompleteBooking(Guid userId, Guid bookingId);
}