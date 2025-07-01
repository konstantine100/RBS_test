using RBS.CORE;
using RBS.DTOs;
using RBS.Helpers;
using RBS.Models;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<List<LayoutByHour>>> GetReservationsByHour (Guid spaceId, DateTime Date);
    Task<ApiResponse<List<ReservationBookingDTO>>> MyReservations(Guid userId);
    Task<ApiResponse<ReservationBookingDTO>> GetMyReservationById(Guid userId, Guid reservationId);
    Task<ApiResponse<List<BookingDTO>>> MyBookings(Guid userId);
    Task<ApiResponse<BookingDTO>> GetMyBookingById(Guid userId, Guid bookingId);
    Task<ApiResponse<BookingDTO>> ClosestBookingReminder(Guid userId);
    Task<ApiResponse<List<BookingDTO>>> MyCurrentBookings(Guid userId);
    Task<ApiResponse<List<BookingDTO>>> MyOldBookings(Guid userId);
    Task<ApiResponse<ReservationBookingDTO>> ChooseSpace(Guid userId ,Guid spaceId, AddBooking request, DateTime endDate);
    Task<ApiResponse<ReservationBookingDTO>> ChooseTable(Guid userId ,Guid tableId, AddBooking request);
    Task<ApiResponse<ReservationBookingDTO>> ChooseChair(Guid userId ,Guid chairId, AddBooking request);
    Task<ApiResponse<ReservationBookingDTO>> ChooseAnotherSpace(Guid userId, Guid reservationId, Guid spaceId);
    Task<ApiResponse<ReservationBookingDTO>> ChooseAnotherTable(Guid userId, Guid reservationId, Guid tableId);
    Task<ApiResponse<ReservationBookingDTO>> ChooseAnotherChair(Guid userId, Guid reservationId, Guid chairId);
    Task<ApiResponse<BookingDTO>> CompleteBooking(Guid userId, Guid reservationId);
    Task<ApiResponse<ReservationBookingDTO>> RemoveReservationSpace(Guid userId, Guid bookingId, Guid spaceId);
    Task<ApiResponse<ReservationBookingDTO>> RemoveReservationTable(Guid userId, Guid bookingId, Guid tableId);
    Task<ApiResponse<ReservationBookingDTO>> RemoveReservationChair(Guid userId, Guid bookingId, Guid chairId);
    Task<ApiResponse<ReservationBookingDTO>> RemoveReservation(Guid userId, Guid reservationId);
    Task<ApiResponse<BookingDTO>> CancelBooking(Guid userId, Guid bookingId);
}