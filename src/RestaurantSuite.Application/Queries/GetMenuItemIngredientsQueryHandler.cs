using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Handler for retrieving ingredients for a menu item
/// </summary>
public class GetMenuItemIngredientsQueryHandler : IRequestHandler<GetMenuItemIngredientsQuery, IEnumerable<MenuItemIngredientDto>>
{
    private readonly IMenuItemIngredientRepository _ingredientRepository;

    public GetMenuItemIngredientsQueryHandler(IMenuItemIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<IEnumerable<MenuItemIngredientDto>> Handle(GetMenuItemIngredientsQuery request, CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientRepository.GetByMenuItemIdAsync(request.MenuItemId, cancellationToken);

        return ingredients.Select(i => new MenuItemIngredientDto
        {
            Id = i.Id,
            IngredientName = i.IngredientName,
            QuantityInGrams = i.QuantityInGrams,
            IsMainIngredient = i.IsMainIngredient,
            DisplayOrder = i.DisplayOrder
        });
    }
}
