using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to delete a menu item
/// </summary>
public class DeleteMenuItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
