using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetRestaurantByIdQuery : IRequest<RestaurantDto?>
{
    public Guid Id { get; set; }
}
