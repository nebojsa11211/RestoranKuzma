using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Infrastructure.EF.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .Include(i => i.Order)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
    }

    public async Task<Invoice?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .FirstOrDefaultAsync(i => i.OrderId == orderId);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .Where(i => i.IssuedDate >= from && i.IssuedDate <= to)
            .OrderByDescending(i => i.IssuedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status)
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByCustomerEmailAsync(string customerEmail)
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .Where(i => i.CustomerEmail == customerEmail)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        var currentYear = DateTime.UtcNow.Year;
        var count = await GetInvoiceCountForYearAsync(currentYear);
        var nextNumber = count + 1;
        return $"INV-{currentYear}-{nextNumber:D5}";
    }

    public async Task<int> GetInvoiceCountForYearAsync(int year)
    {
        var yearStart = new DateTime(year, 1, 1);
        var yearEnd = new DateTime(year, 12, 31, 23, 59, 59);

        return await _context.Invoices
            .Where(i => i.CreatedAt >= yearStart && i.CreatedAt <= yearEnd)
            .CountAsync();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
    }

    public void Update(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
    }

    public void Delete(Invoice invoice)
    {
        _context.Invoices.Remove(invoice);
    }
}
