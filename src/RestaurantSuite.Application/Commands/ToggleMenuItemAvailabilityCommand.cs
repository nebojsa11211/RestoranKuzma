using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to toggle menu item availability
/// </summary>
public class ToggleMenuItemAvailabilityCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public bool IsAvailable { get; set; }
}
