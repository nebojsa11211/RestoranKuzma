using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to create a new menu item
/// </summary>
public class CreateMenuItemCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
}
