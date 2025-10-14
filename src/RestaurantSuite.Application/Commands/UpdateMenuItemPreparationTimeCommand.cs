using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to update menu item preparation time
/// </summary>
public class UpdateMenuItemPreparationTimeCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public int? PreparationTimeMinutes { get; set; }
}
