namespace RBS.Services.Interfaces;

public interface ILayoutNotificationService
{
    Task NotifyLayoutChanged(int spaceId, string changeType, object? additionalData = null);
    Task NotifyLayoutChanged(IEnumerable<int> spaceIds, string changeType, object? additionalData = null);
}