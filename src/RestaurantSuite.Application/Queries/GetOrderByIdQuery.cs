using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    public Guid Id { get; set; }
}
