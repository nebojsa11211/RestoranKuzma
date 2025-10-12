using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to get only available menu items
/// </summary>
public class GetAvailableMenuItemsQuery : IRequest<List<MenuItemDto>>
{
}
