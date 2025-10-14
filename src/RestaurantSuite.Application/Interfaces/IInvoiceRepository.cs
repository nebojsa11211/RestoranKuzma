using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);
    Task<Invoice?> GetByOrderIdAsync(Guid orderId);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status);
    Task<IEnumerable<Invoice>> GetByCustomerEmailAsync(string customerEmail);
    Task<string> GetNextInvoiceNumberAsync();
    Task<int> GetInvoiceCountForYearAsync(int year);
    Task AddAsync(Invoice invoice);
    void Update(Invoice invoice);
    void Delete(Invoice invoice);
}
