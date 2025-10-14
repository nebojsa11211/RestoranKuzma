using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Query to retrieve all recipes with their ingredients
/// </summary>
public class GetAllRecipesQuery : IRequest<List<RecipeDto>>
{
    /// <summary>
    /// Include inactive recipes (default: false)
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
}
