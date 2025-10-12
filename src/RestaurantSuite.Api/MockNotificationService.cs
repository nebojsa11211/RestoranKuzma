using RestaurantSuite.Application.Interfaces;

public class MockNotificationService : INotificationService
{
    public Task NotifyNewOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Mock notification: New order {orderId}");
        return Task.CompletedTask;
    }

    public Task NotifyOrderStatusChangedAsync(Guid orderId, string status, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Mock notification: Order {orderId} status changed to {status}");
        return Task.CompletedTask;
    }

    public Task NotifyTableStatusChangedAsync(Guid tableId, string status, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Mock notification: Table {tableId} status changed to {status}");
        return Task.CompletedTask;
    }
}
