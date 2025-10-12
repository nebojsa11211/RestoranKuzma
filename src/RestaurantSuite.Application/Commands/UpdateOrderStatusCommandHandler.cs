using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new InvalidOperationException($"Order {request.OrderId} not found");

        switch (request.Action.ToLower())
        {
            case "confirm":
                order.ConfirmOrder();
                break;
            case "start":
                order.StartPreparation();
                break;
            case "complete":
                order.CompletePreparation();
                break;
            case "serve":
                order.ServeOrder();
                break;
            case "finish":
                order.CompleteOrder();
                break;
            case "cancel":
                order.CancelOrder();
                break;
            default:
                throw new ArgumentException($"Unknown action: {request.Action}");
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyOrderStatusChangedAsync(order.Id, order.Status.ToString(), cancellationToken);

        return Unit.Value;
    }
}
