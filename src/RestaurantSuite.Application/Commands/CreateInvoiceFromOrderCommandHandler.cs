using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Commands;

public class CreateInvoiceFromOrderCommandHandler : IRequestHandler<CreateInvoiceFromOrderCommand, Guid>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvoiceFromOrderCommandHandler(
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateInvoiceFromOrderCommand request, CancellationToken cancellationToken)
    {
        // Check if invoice already exists for this order
        var existingInvoice = await _invoiceRepository.GetByOrderIdAsync(request.OrderId);
        if (existingInvoice != null)
        {
            throw new InvalidOperationException($"Invoice already exists for order {request.OrderId}");
        }

        // Get order details
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        if (order == null)
        {
            throw new InvalidOperationException($"Order {request.OrderId} not found");
        }

        // Generate invoice number
        var invoiceNumber = await _invoiceRepository.GetNextInvoiceNumberAsync();

        // Create invoice
        var invoice = Invoice.Create(
            invoiceNumber,
            request.OrderId,
            request.TaxRate,
            request.Currency,
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone,
            request.BillingAddress,
            request.Notes
        );

        // Add order items to invoice
        foreach (var orderItem in order.OrderItems)
        {
            invoice.AddItem(
                orderItem.MenuItem?.Name ?? "Unknown Item",
                orderItem.Quantity,
                orderItem.UnitPrice,
                orderItem.Id
            );
        }

        // Add tip if provided
        if (request.TipAmount > 0)
        {
            invoice.SetTip(request.TipAmount);
        }

        // Auto-issue if requested
        if (request.AutoIssue)
        {
            invoice.MarkAsIssued();
        }

        await _invoiceRepository.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }
}
