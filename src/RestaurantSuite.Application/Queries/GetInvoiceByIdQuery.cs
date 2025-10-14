using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetInvoiceByIdQuery : IRequest<InvoiceDto?>
{
    public Guid Id { get; set; }
}
