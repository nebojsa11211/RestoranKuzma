using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler for updating menu item details
/// </summary>
public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMenuItemCommandHandler(
        IMenuItemRepository menuItemRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.Id} not found");

        // Verify category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
            throw new ArgumentException($"Category with ID {request.CategoryId} not found", nameof(request.CategoryId));

        menuItem.UpdateDetails(request.Name, request.Description, request.CategoryId);

        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            menuItem.SetImageUrl(request.ImageUrl);
        }

        _menuItemRepository.Update(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
