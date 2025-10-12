using Microsoft.AspNetCore.SignalR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Notifications.Hubs;

namespace RestaurantSuite.Notifications.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<RestaurantHub> _hubContext;

    public SignalRNotificationService(IHubContext<RestaurantHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"order_{orderId}")
            .SendAsync("NewOrder", new { OrderId = orderId }, cancellationToken);
    }

    public async Task NotifyOrderStatusChangedAsync(Guid orderId, string status, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"order_{orderId}")
            .SendAsync("OrderStatusChanged", new { OrderId = orderId, Status = status }, cancellationToken);
    }

    public async Task NotifyTableStatusChangedAsync(Guid tableId, string status, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All
            .SendAsync("TableStatusChanged", new { TableId = tableId, Status = status }, cancellationToken);
    }
}
