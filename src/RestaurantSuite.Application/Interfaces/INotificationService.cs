namespace RestaurantSuite.Application.Interfaces;

public interface INotificationService
{
    Task NotifyNewOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task NotifyOrderStatusChangedAsync(Guid orderId, string status, CancellationToken cancellationToken = default);
    Task NotifyTableStatusChangedAsync(Guid tableId, string status, CancellationToken cancellationToken = default);
}
