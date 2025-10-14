using MediatR;

namespace RestaurantSuite.Application.Commands;

public class CancelInvoiceCommand : IRequest<Unit>
{
    public Guid InvoiceId { get; set; }
}
