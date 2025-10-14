using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Handler for getting menu items with full recipe details
/// </summary>
public class GetMenuWithRecipesQueryHandler : IRequestHandler<GetMenuWithRecipesQuery, List<MenuItemWithIngredientsDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetMenuWithRecipesQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<List<MenuItemWithIngredientsDto>> Handle(GetMenuWithRecipesQuery request, CancellationToken cancellationToken)
    {
        var menuItems = await _menuItemRepository.GetAvailableWithIngredientsAsync(request.RestaurantId, cancellationToken);

        // Filter by category if specified
        if (request.CategoryId.HasValue)
        {
            menuItems = menuItems.Where(m => m.CategoryId == request.CategoryId.Value);
        }

        return menuItems.Select(m => new MenuItemWithIngredientsDto
        {
            Id = m.Id,
            CategoryId = m.CategoryId,
            Name = m.Name,
            Description = m.Description,
            Price = m.Price,
            ImageUrl = m.ImageUrl,
            IsAvailable = m.IsAvailable,
            PreparationTimeMinutes = m.PreparationTimeMinutes,
            CategoryName = m.Category?.Name ?? string.Empty,
            Ingredients = m.Ingredients
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new MenuItemIngredientDto
                {
                    Id = i.Id,
                    MenuItemId = i.MenuItemId,
                    IngredientName = i.IngredientName,
                    QuantityInGrams = i.QuantityInGrams,
                    IsMainIngredient = i.IsMainIngredient,
                    DisplayOrder = i.DisplayOrder,
                    CreatedAt = i.CreatedAt
                }).ToList(),
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        }).ToList();
    }
}
