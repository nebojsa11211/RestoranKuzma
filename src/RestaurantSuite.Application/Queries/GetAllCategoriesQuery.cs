using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to get all categories
/// </summary>
public class GetAllCategoriesQuery : IRequest<List<CategoryDto>>
{
}
