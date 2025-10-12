using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to get all menu items with optional category filter
/// </summary>
public class GetAllMenuItemsQuery : IRequest<List<MenuItemDto>>
{
    public Guid? CategoryId { get; set; }
    public bool? AvailableOnly { get; set; }
}
