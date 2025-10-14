using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to get menu items with simplified ingredients (Guest view)
/// </summary>
public class GetGuestMenuQuery : IRequest<List<GuestMenuItemDto>>
{
    public Guid? CategoryId { get; set; }
    public Guid RestaurantId { get; set; }
}
