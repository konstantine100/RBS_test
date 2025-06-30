using RBS.CORE;
using RBS.DTOs;
using RBS.Helpers;
using RBS.Models;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IBookingService
{
    ApiResponse<List<LayoutByHour>> GetReservationsByHour (Guid spaceId, DateTime Date);
    ApiResponse<ReservationBookingDTO> ChooseSpace(Guid userId ,Guid spaceId, AddBooking request, DateTime endDate);
    ApiResponse<ReservationBookingDTO> ChooseTable(Guid userId ,Guid tableId, AddBooking request);
    ApiResponse<ReservationBookingDTO> ChooseChair(Guid userId ,Guid chairId, AddBooking request);
    ApiResponse<ReservationBookingDTO> ChooseAnotherSpace(Guid userId, Guid reservationId, Guid spaceId);
    ApiResponse<ReservationBookingDTO> ChooseAnotherTable(Guid userId, Guid reservationId, Guid tableId);
    ApiResponse<ReservationBookingDTO> ChooseAnotherChair(Guid userId, Guid reservationId, Guid chairId);
    ApiResponse<BookingDTO> CompleteBooking(Guid userId, Guid reservationId);
    ApiResponse<ReservationBookingDTO> RemoveReservationSpace(Guid userId, Guid bookingId, Guid spaceId);
    ApiResponse<ReservationBookingDTO> RemoveReservationTable(Guid userId, Guid bookingId, Guid tableId);
    ApiResponse<ReservationBookingDTO> RemoveReservationChair(Guid userId, Guid bookingId, Guid chairId);
    ApiResponse<ReservationBookingDTO> RemoveReservation(Guid userId, Guid reservationId);
    ApiResponse<BookingDTO> CancelBooking(Guid userId, Guid bookingId);
}