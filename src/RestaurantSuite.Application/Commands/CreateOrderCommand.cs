using MediatR;

namespace RestaurantSuite.Application.Commands;

public class CreateOrderCommand : IRequest<Guid>
{
    public Guid TableId { get; set; }
    public Guid WaiterId { get; set; }
    public Guid? GuestId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SpecialInstructions { get; set; }
}
