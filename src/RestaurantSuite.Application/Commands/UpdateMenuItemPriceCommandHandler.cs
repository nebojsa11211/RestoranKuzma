using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler for updating menu item price
/// </summary>
public class UpdateMenuItemPriceCommandHandler : IRequestHandler<UpdateMenuItemPriceCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMenuItemPriceCommandHandler(
        IMenuItemRepository menuItemRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateMenuItemPriceCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.Id} not found");

        menuItem.UpdatePrice(request.Price);

        _menuItemRepository.Update(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
