using MediatR;

namespace RestaurantSuite.Application.Commands;

public class RecordInvoicePaymentCommand : IRequest<Unit>
{
    public Guid InvoiceId { get; set; }
    public Guid PaymentId { get; set; }
}
