using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface ISpaceReservationService
{
    Task<ApiResponse<SpaceReservationDTO>> ChooseSpace(int userId ,int spaceId, AddReservation request, DateTime endDate);
    Task<ApiResponse<List<BookingDTO>>> SpaceBookingForDay(int spaceId, DateTime date);
    Task<ApiResponse<SpaceReservationDTO>> RemoveReservationSpace(int userId, int reservationId);
}