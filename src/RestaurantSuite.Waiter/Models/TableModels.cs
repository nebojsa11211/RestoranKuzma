namespace RestaurantSuite.Waiter.Models;

public enum TableStatus
{
    Available = 0,
    Occupied = 1,
    Reserved = 2
}

public class Table
{
    public Guid Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public TableStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Additional properties for waiter view
    public Guid? CurrentOrderId { get; set; }
    public string? AssignedWaiterName { get; set; }
    public int? GuestCount { get; set; }
    public decimal? CurrentBill { get; set; }
}

public class UpdateTableStatusRequest
{
    public Guid TableId { get; set; }
    public TableStatus NewStatus { get; set; }
}

public class TableResponse
{
    public bool Success { get; set; }
    public Table? Table { get; set; }
    public string? ErrorMessage { get; set; }
}

public class TableListResponse
{
    public bool Success { get; set; }
    public List<Table> Tables { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
