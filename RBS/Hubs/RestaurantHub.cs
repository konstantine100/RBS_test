using Microsoft.AspNetCore.SignalR;

namespace RBS.Hubs;

public class RestaurantHub : Hub
{
    public async Task JoinSpaceGroup(int spaceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Space_{spaceId}");
    }

    public async Task LeaveSpaceGroup(int spaceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Space_{spaceId}");
    }
}