using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid TableId { get; private set; }
    public Guid WaiterId { get; private set; }
    public Guid? GuestId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    // Navigation properties
    public Table Table { get; private set; }
    public User Waiter { get; private set; }
    public User? Guest { get; private set; }

    private Order() { }

    public static Order Create(Guid tableId, Guid waiterId, Guid? guestId = null)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            TableId = tableId,
            WaiterId = waiterId,
            GuestId = guestId,
            Status = OrderStatus.Pending,
            TotalAmount = 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddItem(Guid menuItemId, int quantity, decimal unitPrice, string? specialInstructions)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        var orderItem = OrderItem.Create(Id, menuItemId, quantity, unitPrice, specialInstructions);
        _orderItems.Add(orderItem);
        RecalculateTotalAmount();
    }

    public void RemoveItem(Guid orderItemId)
    {
        var item = _orderItems.FirstOrDefault(i => i.Id == orderItemId);
        if (item != null)
        {
            _orderItems.Remove(item);
            RecalculateTotalAmount();
        }
    }

    public void UpdateItemQuantity(Guid orderItemId, int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        var item = _orderItems.FirstOrDefault(i => i.Id == orderItemId);
        if (item != null)
        {
            item.UpdateQuantity(newQuantity);
            RecalculateTotalAmount();
        }
    }

    private void RecalculateTotalAmount()
    {
        TotalAmount = _orderItems.Sum(i => i.Subtotal);
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        Status = OrderStatus.Confirmed;
    }

    public void StartPreparation()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can start preparation");

        Status = OrderStatus.InProgress;
    }

    public void CompletePreparation()
    {
        if (Status != OrderStatus.InProgress)
            throw new InvalidOperationException("Only in-progress orders can be completed");

        Status = OrderStatus.Ready;
    }

    public void ServeOrder()
    {
        if (Status != OrderStatus.Ready)
            throw new InvalidOperationException("Only ready orders can be served");

        Status = OrderStatus.Served;
    }

    public void CompleteOrder()
    {
        if (Status != OrderStatus.Served)
            throw new InvalidOperationException("Only served orders can be completed");

        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void CancelOrder()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed orders");

        Status = OrderStatus.Cancelled;
    }
}
