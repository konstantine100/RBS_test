using RBS.CORE;

namespace RBS.Services.Interfaces;

public interface ILayoutService
{
    Task<ApiResponse<List<LayoutByHour>>> GetLayoutByHour (int spaceId, DateTime Date);
}