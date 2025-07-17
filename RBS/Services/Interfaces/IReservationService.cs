using RBS.CORE;
using RBS.DTOs;

namespace RBS.Services.Interfaces;

public interface IReservationService
{
    Task<ApiResponse<List<OverallReservations>>> MyReservations(int userId);
}