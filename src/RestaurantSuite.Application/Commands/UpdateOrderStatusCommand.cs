using MediatR;

namespace RestaurantSuite.Application.Commands;

public class UpdateOrderStatusCommand : IRequest<Unit>
{
    public Guid OrderId { get; set; }
    public string Action { get; set; } = string.Empty; // "confirm", "start", "complete", "serve", "finish", "cancel"
}
