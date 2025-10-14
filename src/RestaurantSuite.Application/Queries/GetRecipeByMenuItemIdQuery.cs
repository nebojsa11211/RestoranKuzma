using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to retrieve a recipe for a specific menu item
/// </summary>
public class GetRecipeByMenuItemIdQuery : IRequest<RecipeDto?>
{
    public Guid MenuItemId { get; set; }
}
