using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetInvoiceByOrderIdQuery : IRequest<InvoiceDto?>
{
    public Guid OrderId { get; set; }
}
