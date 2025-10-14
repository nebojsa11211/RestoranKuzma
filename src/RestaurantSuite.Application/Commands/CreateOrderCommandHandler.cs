using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Items == null || !request.Items.Any())
            throw new ArgumentException("Order must contain at least one item");

        var order = Order.Create(request.TableId, request.WaiterId, request.GuestId);

        foreach (var item in request.Items)
        {
            var orderItem = order.AddItem(item.MenuItemId, item.Quantity, item.UnitPrice, item.SpecialInstructions);

            // Add customizations to the order item
            foreach (var customization in item.Customizations)
            {
                orderItem.AddCustomization(
                    customization.IngredientName,
                    customization.CustomizationType,
                    customization.Notes);
            }
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyNewOrderAsync(order.Id, cancellationToken);

        return order.Id;
    }
}
