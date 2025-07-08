using RBS.CORE;
using RBS.DTOs;
using RBS.Models;

namespace RBS.Services.Interfaces;

public interface IAdminService
{
    Task<ApiResponse<UserDTO>> MakeUserHost(int userId, int restaurantId);
    Task<ApiResponse<List<UserDTO>>> SeeHosts(int restaurantId);
    Task<ApiResponse<List<WalkIn>>> SeeHostWalkIns(int restaurantId, int hostId);
    Task<ApiResponse<UserDTO>> DemoteHost(int restaurantId, int hostId);
}