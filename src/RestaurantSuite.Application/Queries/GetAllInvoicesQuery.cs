using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetAllInvoicesQuery : IRequest<IEnumerable<InvoiceDto>>
{
}
