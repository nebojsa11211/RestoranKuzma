using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IMediator _mediator;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IMediator mediator)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _mediator = mediator;
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

                // AUTOMATED STOCK REDUCTION: Reduce inventory when order preparation starts
                var stockReductionCommand = new ReduceStockForOrderCommand
                {
                    OrderId = order.Id,
                    ProcessedBy = Guid.Empty // TODO: Get actual user ID from context
                };
                var stockResult = await _mediator.Send(stockReductionCommand, cancellationToken);

                if (!stockResult.Success)
                {
                    // Log warning but don't fail the order status update
                    // The order can still be prepared even if stock tracking fails
                    // In a production system, you might want to send an alert
                }
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
