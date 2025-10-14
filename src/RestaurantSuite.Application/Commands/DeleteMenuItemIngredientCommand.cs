using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to delete a menu item ingredient
/// </summary>
public class DeleteMenuItemIngredientCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
