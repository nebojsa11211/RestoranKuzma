using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetAllRestaurantsQuery : IRequest<List<RestaurantDto>>
{
    // No parameters needed for single-restaurant architecture
}
