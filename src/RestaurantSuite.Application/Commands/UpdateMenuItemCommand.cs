using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to update menu item details
/// </summary>
public class UpdateMenuItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
}
