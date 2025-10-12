namespace RestaurantSuite.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid TableId { get; set; }
    public Guid WaiterId { get; set; }
    public Guid? GuestId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public string CustomerName { get; set; } = string.Empty;
}
