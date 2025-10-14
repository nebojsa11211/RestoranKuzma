using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
        if (invoice == null)
            return null;

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            OrderId = invoice.OrderId,
            PaymentId = invoice.PaymentId,
            Status = invoice.Status,
            IssuedDate = invoice.IssuedDate,
            DueDate = invoice.DueDate,
            PaidDate = invoice.PaidDate,
            SubtotalAmount = invoice.SubtotalAmount,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            TipAmount = invoice.TipAmount,
            TotalAmount = invoice.TotalAmount,
            Currency = invoice.Currency,
            TaxRate = invoice.TaxRate,
            Notes = invoice.Notes,
            CustomerName = invoice.CustomerName,
            CustomerEmail = invoice.CustomerEmail,
            CustomerPhone = invoice.CustomerPhone,
            BillingAddress = invoice.BillingAddress,
            DiscountReason = invoice.DiscountReason,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt,
            InvoiceItems = invoice.InvoiceItems.Select(item => new InvoiceItemDto
            {
                Id = item.Id,
                InvoiceId = item.InvoiceId,
                OrderItemId = item.OrderItemId,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.Subtotal,
                TaxAmount = item.TaxAmount,
                TotalAmount = item.TotalAmount
            }).ToList()
        };
    }
}
