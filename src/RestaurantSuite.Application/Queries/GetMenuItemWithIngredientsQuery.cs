using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to get a menu item with its complete recipe (ingredients)
/// </summary>
public class GetMenuItemWithIngredientsQuery : IRequest<MenuItemWithIngredientsDto?>
{
    public Guid Id { get; set; }
}
