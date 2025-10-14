namespace RestaurantSuite.Application.Interfaces;

public interface IInvoiceService
{
    Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId);
    Task<string> GenerateInvoiceHtmlAsync(Guid invoiceId);
}
