using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Handler for retrieving a menu item with its complete recipe
/// </summary>
public class GetMenuItemWithIngredientsQueryHandler : IRequestHandler<GetMenuItemWithIngredientsQuery, MenuItemWithIngredientsDto?>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetMenuItemWithIngredientsQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<MenuItemWithIngredientsDto?> Handle(GetMenuItemWithIngredientsQuery request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdWithIngredientsAsync(request.Id, cancellationToken);

        if (menuItem == null)
            return null;

        return new MenuItemWithIngredientsDto
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Description = menuItem.Description,
            Price = menuItem.Price,
            CategoryId = menuItem.CategoryId,
            ImageUrl = menuItem.ImageUrl,
            IsAvailable = menuItem.IsAvailable,
            PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
            Ingredients = menuItem.Ingredients
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new MenuItemIngredientDto
                {
                    Id = i.Id,
                    IngredientName = i.IngredientName,
                    QuantityInGrams = i.QuantityInGrams,
                    IsMainIngredient = i.IsMainIngredient,
                    DisplayOrder = i.DisplayOrder
                })
                .ToList()
        };
    }
}
