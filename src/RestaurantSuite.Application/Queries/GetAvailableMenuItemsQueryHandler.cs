using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Handler for getting only available menu items
/// </summary>
public class GetAvailableMenuItemsQueryHandler : IRequestHandler<GetAvailableMenuItemsQuery, List<MenuItemDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetAvailableMenuItemsQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<List<MenuItemDto>> Handle(GetAvailableMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var menuItems = await _menuItemRepository.GetAvailableByRestaurantIdAsync(Guid.Empty, cancellationToken);

        return menuItems.Select(m => new MenuItemDto
        {
            Id = m.Id,
            CategoryId = m.CategoryId,
            Name = m.Name,
            Description = m.Description,
            Price = m.Price,
            ImageUrl = m.ImageUrl,
            IsAvailable = m.IsAvailable,
            CategoryName = m.Category?.Name ?? string.Empty,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        }).ToList();
    }
}
