using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface ITableReservationService
{
    Task<ApiResponse<TableReservationDTO>> ChooseTable(int userId ,int tableId, AddReservation request);
    Task<ApiResponse<List<BookingDTO>>> TableBookingForDay(int tableId, DateTime date);
    Task<ApiResponse<TableReservationDTO>> RemoveReservationTable(int userId, int reservationId);
}