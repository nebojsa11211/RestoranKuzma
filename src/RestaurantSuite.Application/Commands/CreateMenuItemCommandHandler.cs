using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler for creating a new menu item
/// </summary>
public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Guid>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMenuItemCommandHandler(
        IMenuItemRepository menuItemRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"CreateMenuItemCommandHandler: Creating menu item with Name={request.Name}, Price={request.Price}, CategoryId={request.CategoryId}");

            // Verify category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                Console.WriteLine($"Category with ID {request.CategoryId} not found");
                throw new ArgumentException($"Category with ID {request.CategoryId} not found", nameof(request.CategoryId));
            }

            Console.WriteLine("Creating MenuItem entity...");
            var menuItem = MenuItem.Create(request.Name, request.Description, request.Price, request.CategoryId);

            // Set UpdatedAt to the same as CreatedAt for new items
            menuItem.UpdateDetails(request.Name, request.Description, request.CategoryId);

            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                menuItem.SetImageUrl(request.ImageUrl);
            }

            Console.WriteLine("Adding menu item to repository...");
            await _menuItemRepository.AddAsync(menuItem, cancellationToken);
            
            Console.WriteLine("Saving changes...");
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"Menu item created successfully with ID: {menuItem.Id}");
            return menuItem.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateMenuItemCommandHandler: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
