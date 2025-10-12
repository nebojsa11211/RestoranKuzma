namespace RestaurantSuite.Chef.Models;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    InProgress = 2,
    Ready = 3,
    Served = 4,
    Completed = 5,
    Cancelled = 6
}

public class Order
{
    public Guid Id { get; set; }
    public Guid TableId { get; set; }
    public Guid WaiterId { get; set; }
    public Guid? GuestId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();

    // Navigation properties for display
    public string? TableNumber { get; set; }
    public string? WaiterName { get; set; }
    public string? GuestName { get; set; }
}

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public string? SpecialInstructions { get; set; }

    // Navigation properties for display
    public string? MenuItemName { get; set; }
    public string? MenuItemDescription { get; set; }
}

public class UpdateOrderStatusRequest
{
    public Guid OrderId { get; set; }
    public string Action { get; set; } = string.Empty;
}

public class OrderResponse
{
    public bool Success { get; set; }
    public Order? Order { get; set; }
    public string? ErrorMessage { get; set; }
}

public class OrderListResponse
{
    public bool Success { get; set; }
    public List<Order> Orders { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

// Chef-specific models
public class KitchenStats
{
    public int ConfirmedOrders { get; set; }
    public int InProgressOrders { get; set; }
    public int ReadyOrders { get; set; }
    public int CompletedToday { get; set; }
}

public class KitchenStatsResponse
{
    public bool Success { get; set; }
    public KitchenStats? Stats { get; set; }
    public string? ErrorMessage { get; set; }
}
