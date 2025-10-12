using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler for toggling menu item availability
/// </summary>
public class ToggleMenuItemAvailabilityCommandHandler : IRequestHandler<ToggleMenuItemAvailabilityCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleMenuItemAvailabilityCommandHandler(
        IMenuItemRepository menuItemRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ToggleMenuItemAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.Id} not found");

        if (request.IsAvailable)
            menuItem.MarkAsAvailable();
        else
            menuItem.MarkAsUnavailable();

        _menuItemRepository.Update(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
