using MediatR;

namespace RestaurantSuite.Application.Commands;

public class IssueInvoiceCommand : IRequest<Unit>
{
    public Guid InvoiceId { get; set; }
}
