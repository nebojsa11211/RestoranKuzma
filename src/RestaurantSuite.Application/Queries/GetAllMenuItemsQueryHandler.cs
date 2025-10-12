using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

/// <summary>
/// Handler for getting all menu items with optional filters
/// </summary>
public class GetAllMenuItemsQueryHandler : IRequestHandler<GetAllMenuItemsQuery, List<MenuItemDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetAllMenuItemsQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<List<MenuItemDto>> Handle(GetAllMenuItemsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.MenuItem> menuItems;

        if (request.CategoryId.HasValue)
        {
            menuItems = await _menuItemRepository.GetByCategoryIdAsync(request.CategoryId.Value, cancellationToken);
        }
        else if (request.AvailableOnly == true)
        {
            menuItems = await _menuItemRepository.GetAvailableByRestaurantIdAsync(Guid.Empty, cancellationToken);
        }
        else
        {
            menuItems = await _menuItemRepository.GetByRestaurantIdAsync(Guid.Empty, cancellationToken);
        }

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
