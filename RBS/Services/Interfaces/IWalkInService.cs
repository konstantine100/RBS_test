using RBS.CORE;
using RBS.DTOs;

namespace RBS.Services.Interfaces;

public interface IWalkInService
{
    Task<ApiResponse<WalkInDTO>> AddWalkInTable(int hostId, int tableId);
    Task<ApiResponse<WalkInDTO>> AddWalkInChair(int hostId, int chairId);
    Task<ApiResponse<List<WalkInDTO>>> GetMyWalkIns(int hostId);
    Task<ApiResponse<WalkInDTO>> FinishWalkIn(int hostId, int walkInId);
}