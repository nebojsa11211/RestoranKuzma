using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

public class CancelInvoiceCommandHandler : IRequestHandler<CancelInvoiceCommand, Unit>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
        if (invoice == null)
        {
            throw new InvalidOperationException($"Invoice {request.InvoiceId} not found");
        }

        invoice.Cancel();
        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
