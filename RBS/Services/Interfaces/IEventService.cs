using RBS.CORE;
using RBS.DTOs;
using RBS.Requests;

namespace RBS.Services.Interfaces;

public interface IEventService
{
    Task<ApiResponse<EventDTO>> CreateEvent(int adminId, AddEvent request);
    Task<ApiResponse<List<EventDTO>>> GetActiveEvents(int restaurantId);
    Task<ApiResponse<List<EventDTO>>> GetPastEvents(int restaurantId);
    Task<ApiResponse<List<EventDTO>>> SearchSortEvents(int restaurantId, string? searchString, string? sortBy);
    Task<ApiResponse<EventDTO>> GetEventById(int eventId);
    Task<ApiResponse<EventDTO>> UpdateEvent(int adminId, int eventId, string changeParameter, string changeTo);
    Task<ApiResponse<EventDTO>> DeleteEvent(int adminId, int eventId);
    
    
}