using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to get ingredients for a specific menu item
/// </summary>
public class GetMenuItemIngredientsQuery : IRequest<IEnumerable<MenuItemIngredientDto>>
{
    public Guid MenuItemId { get; set; }
}
