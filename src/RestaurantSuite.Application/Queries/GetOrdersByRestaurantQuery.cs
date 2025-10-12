using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetOrdersByRestaurantQuery : IRequest<IEnumerable<OrderDto>>
{
    // No parameters needed since there's only one restaurant in the system
}
