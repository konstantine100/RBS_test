using Microsoft.AspNetCore.SignalR;
using RBS.Hubs;
using RBS.Services.Interfaces;

namespace RBS.Services.Implementation;

public class LayoutNotificationService : ILayoutNotificationService
{
    private readonly IHubContext<RestaurantHub> _hubContext;
    private readonly ILogger<LayoutNotificationService> _logger;

    public LayoutNotificationService(IHubContext<RestaurantHub> hubContext, ILogger<LayoutNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyLayoutChanged(int spaceId, string changeType, object? additionalData = null)
    {
        try
        {
            var notification = new
            {
                spaceId = spaceId,
                changeType = changeType,
                timestamp = DateTime.UtcNow,
                data = additionalData
            };

            await _hubContext.Clients.Group($"Space_{spaceId}")
                .SendAsync("LayoutChanged", notification);

            _logger.LogInformation($"Layout change notification sent for space {spaceId}: {changeType}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send layout change notification for space {spaceId}");
        }
    }

    public async Task NotifyLayoutChanged(IEnumerable<int> spaceIds, string changeType, object? additionalData = null)
    {
        foreach (var spaceId in spaceIds)
        {
            await NotifyLayoutChanged(spaceId, changeType, additionalData);
        }
    }
}