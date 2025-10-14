using Microsoft.AspNetCore.SignalR;

namespace RestaurantSuite.Notifications.Hubs;

public class RestaurantHub : Hub
{
    public async Task JoinRestaurantGroup(string restaurantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"restaurant_{restaurantId}");
    }

    public async Task LeaveRestaurantGroup(string restaurantId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"restaurant_{restaurantId}");
    }

    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"order_{orderId}");
    }

    public async Task LeaveOrderGroup(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order_{orderId}");
    }

    // Server-side methods for broadcasting events
    public async Task NotifyOrderCreated(string restaurantId, object order)
    {
        await Clients.Group($"restaurant_{restaurantId}").SendAsync("OrderCreated", order);
    }

    public async Task NotifyOrderStatusChanged(string restaurantId, string orderId, string status, DateTime timestamp)
    {
        await Clients.Group($"restaurant_{restaurantId}").SendAsync("OrderStatusChanged", orderId, status, timestamp);
    }

    public async Task NotifyTableStatusChanged(string restaurantId, string tableId, string status, DateTime timestamp)
    {
        await Clients.Group($"restaurant_{restaurantId}").SendAsync("TableStatusChanged", tableId, status, timestamp);
    }

    public async Task NotifyPaymentProcessed(string restaurantId, string orderId, object payment)
    {
        await Clients.Group($"restaurant_{restaurantId}").SendAsync("PaymentProcessed", orderId, payment);
    }

    public async Task SendNotification(string restaurantId, object notification)
    {
        await Clients.Group($"restaurant_{restaurantId}").SendAsync("NotificationReceived", notification);
    }
}
