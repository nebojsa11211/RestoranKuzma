using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler for adding an ingredient to a menu item
/// </summary>
public class AddMenuItemIngredientCommandHandler : IRequestHandler<AddMenuItemIngredientCommand, Guid>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddMenuItemIngredientCommandHandler(
        IMenuItemRepository menuItemRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddMenuItemIngredientCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.MenuItemId} not found");

        var ingredientId = menuItem.AddIngredient(
            request.IngredientName,
            request.QuantityInGrams,
            request.IsMainIngredient,
            request.DisplayOrder);

        _menuItemRepository.Update(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ingredientId;
    }
}
