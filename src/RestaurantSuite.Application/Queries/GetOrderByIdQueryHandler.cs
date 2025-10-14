using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(request.Id, cancellationToken);

        if (order == null)
            return null;

        return new OrderDto
        {
            Id = order.Id,
            TableId = order.TableId,
            WaiterId = order.WaiterId,
            GuestId = order.GuestId,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            CompletedAt = order.CompletedAt,
            Items = order.OrderItems.Select(item => new OrderItemDto
            {
                Id = item.Id,
                MenuItemId = item.MenuItemId,
                MenuItemName = item.MenuItem?.Name ?? string.Empty,
                Quantity = item.Quantity,
                Price = item.UnitPrice,
                SpecialInstructions = item.SpecialInstructions
            }).ToList()
        };
    }
}
