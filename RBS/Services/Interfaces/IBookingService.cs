using RBS.CORE;
using RBS.DTOs;
using RBS.Helpers;
using RBS.Models;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<BookingDTO>> CompleteBooking(int userId, int reservationId);
    Task<ApiResponse<List<LayoutByHour>>> GetReservationsByHour (int spaceId, DateTime Date);
    Task<ApiResponse<List<ReservationBookingDTO>>> MyReservations(int userId);
    Task<ApiResponse<ReservationBookingDTO>> GetMyReservationById(int userId, int reservationId);
    Task<ApiResponse<List<BookingDTO>>> MyBookings(int userId);
    Task<ApiResponse<BookingDTO>> GetMyBookingById(int userId, int bookingId);
    Task<ApiResponse<BookingDTO>> ClosestBookingReminder(int userId);
    Task<ApiResponse<List<BookingDTO>>> MyCurrentBookings(int userId);
    Task<ApiResponse<List<BookingDTO>>> MyOldBookings(int userId);
    Task<ApiResponse<SpaceReservationDTO>> ChooseSpace(int userId ,int spaceId, AddReservation request, DateTime endDate);
    Task<ApiResponse<TableReservationDTO>> ChooseTable(int userId ,int tableId, AddReservation request);
    Task<ApiResponse<ChairReservationDTO>> ChooseChair(int userId ,int chairId, AddReservation request);
    
    Task<ApiResponse<ReservationBookingDTO>> RemoveReservationSpace(int userId, int bookingId, int spaceId);
    Task<ApiResponse<ReservationBookingDTO>> RemoveReservationTable(int userId, int bookingId, int tableId);
    Task<ApiResponse<ReservationBookingDTO>> RemoveReservationChair(int userId, int bookingId, int chairId);
    Task<ApiResponse<ReservationBookingDTO>> RemoveReservation(int userId, int reservationId);
    Task<ApiResponse<BookingDTO>> CancelBooking(int userId, int bookingId);
}