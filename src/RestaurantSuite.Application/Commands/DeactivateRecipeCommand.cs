using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to deactivate a recipe (soft delete)
/// </summary>
public class DeactivateRecipeCommand : IRequest<Unit>
{
    public Guid MenuItemId { get; set; }
}
