using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler for updating menu item preparation time
/// </summary>
public class UpdateMenuItemPreparationTimeCommandHandler : IRequestHandler<UpdateMenuItemPreparationTimeCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMenuItemPreparationTimeCommandHandler(
        IMenuItemRepository menuItemRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateMenuItemPreparationTimeCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.Id} not found");

        menuItem.SetPreparationTime(request.PreparationTimeMinutes);

        _menuItemRepository.Update(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
