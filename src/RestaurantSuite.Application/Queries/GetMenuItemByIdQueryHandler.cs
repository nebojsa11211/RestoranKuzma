using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Handler for getting a single menu item by ID
/// </summary>
public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, MenuItemDto?>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetMenuItemByIdQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<MenuItemDto?> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (menuItem == null)
            return null;

        return new MenuItemDto
        {
            Id = menuItem.Id,
            CategoryId = menuItem.CategoryId,
            Name = menuItem.Name,
            Description = menuItem.Description,
            Price = menuItem.Price,
            ImageUrl = menuItem.ImageUrl,
            IsAvailable = menuItem.IsAvailable,
            CategoryName = menuItem.Category?.Name ?? string.Empty,
            CreatedAt = menuItem.CreatedAt,
            UpdatedAt = menuItem.UpdatedAt
        };
    }
}
