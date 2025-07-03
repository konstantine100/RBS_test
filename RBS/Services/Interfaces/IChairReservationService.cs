using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IChairReservationService
{
    Task<ApiResponse<ChairReservationDTO>> ChooseChair(int userId ,int chairId, AddReservation request);
    Task<ApiResponse<ChairReservationDTO>> RemoveReservationChair(int userId, int reservationId);
}