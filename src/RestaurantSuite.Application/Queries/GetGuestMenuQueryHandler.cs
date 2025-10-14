using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Handler for getting menu items with simplified ingredient view for guests
/// </summary>
public class GetGuestMenuQueryHandler : IRequestHandler<GetGuestMenuQuery, List<GuestMenuItemDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetGuestMenuQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<List<GuestMenuItemDto>> Handle(GetGuestMenuQuery request, CancellationToken cancellationToken)
    {
        var menuItems = await _menuItemRepository.GetAvailableWithIngredientsAsync(request.RestaurantId, cancellationToken);

        // Filter by category if specified
        if (request.CategoryId.HasValue)
        {
            menuItems = menuItems.Where(m => m.CategoryId == request.CategoryId.Value);
        }

        return menuItems.Select(m => new GuestMenuItemDto
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
            MainIngredients = m.Ingredients
                .Where(i => i.IsMainIngredient)
                .OrderBy(i => i.DisplayOrder)
                .Select(i => i.IngredientName)
                .ToList()
        }).ToList();
    }
}
