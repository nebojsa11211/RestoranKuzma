using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IEnumerable<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAllAsync();

        return invoices.Select(invoice => new InvoiceDto
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
        }).ToList();
    }
}
