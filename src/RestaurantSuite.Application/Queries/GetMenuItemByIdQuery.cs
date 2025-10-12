using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to get a single menu item by ID
/// </summary>
public class GetMenuItemByIdQuery : IRequest<MenuItemDto?>
{
    public Guid Id { get; set; }
}
