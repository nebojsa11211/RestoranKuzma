using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to get menu items with full recipe details (Chef/Admin view)
/// </summary>
public class GetMenuWithRecipesQuery : IRequest<List<MenuItemWithIngredientsDto>>
{
    public Guid? CategoryId { get; set; }
    public Guid RestaurantId { get; set; }
}
