using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to update menu item price
/// </summary>
public class UpdateMenuItemPriceCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
}
