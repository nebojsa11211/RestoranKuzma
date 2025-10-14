using MediatR;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Commands;

public class CreateOrderCommand : IRequest<Guid>
{
    public Guid TableId { get; set; }
    public Guid WaiterId { get; set; }
    public Guid? GuestId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SpecialInstructions { get; set; }
    public List<CreateOrderItemCustomizationRequest> Customizations { get; set; } = new();
}

public class CreateOrderItemCustomizationRequest
{
    public string IngredientName { get; set; } = string.Empty;
    public CustomizationType CustomizationType { get; set; }
    public string? Notes { get; set; }
}
