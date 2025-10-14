using System.Text;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IRestaurantRepository restaurantRepository)
    {
        _invoiceRepository = invoiceRepository;
        _restaurantRepository = restaurantRepository;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId)
    {
        var html = await GenerateInvoiceHtmlAsync(invoiceId);
        // TODO: Convert HTML to PDF using a library like QuestPDF or Puppeteer
        // For now, return HTML as bytes
        return Encoding.UTF8.GetBytes(html);
    }

    public async Task<string> GenerateInvoiceHtmlAsync(Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
        if (invoice == null)
        {
            throw new InvalidOperationException($"Invoice {invoiceId} not found");
        }

        var restaurant = (await _restaurantRepository.GetAllAsync()).FirstOrDefault();
        var restaurantName = restaurant?.Name ?? "Restaurant";
        var restaurantAddress = restaurant?.Address ?? "";

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset='UTF-8'>");
        html.AppendLine("<title>Invoice " + invoice.InvoiceNumber + "</title>");
        html.AppendLine("<style>");
        html.AppendLine(@"
            body { font-family: Arial, sans-serif; margin: 40px; color: #333; }
            .invoice-header { text-align: center; margin-bottom: 30px; border-bottom: 2px solid #667eea; padding-bottom: 20px; }
            .invoice-header h1 { margin: 0; color: #667eea; }
            .invoice-info { display: flex; justify-content: space-between; margin-bottom: 30px; }
            .invoice-info div { flex: 1; }
            .invoice-details { background: #f9fafb; padding: 15px; border-radius: 8px; }
            .invoice-details p { margin: 5px 0; }
            .invoice-items { margin: 30px 0; }
            .invoice-items table { width: 100%; border-collapse: collapse; }
            .invoice-items th, .invoice-items td { padding: 12px; text-align: left; border-bottom: 1px solid #e5e7eb; }
            .invoice-items th { background: #667eea; color: white; font-weight: 600; }
            .invoice-items tr:hover { background: #f3f4f6; }
            .text-right { text-align: right; }
            .totals { margin-top: 20px; float: right; width: 300px; }
            .totals table { width: 100%; }
            .totals td { padding: 8px; }
            .totals .grand-total { font-size: 18px; font-weight: bold; border-top: 2px solid #667eea; padding-top: 10px; }
            .footer { margin-top: 60px; text-align: center; color: #666; font-size: 12px; clear: both; }
        ");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        // Header
        html.AppendLine("<div class='invoice-header'>");
        html.AppendLine($"<h1>{restaurantName}</h1>");
        html.AppendLine($"<p>{restaurantAddress}</p>");
        html.AppendLine("</div>");

        // Invoice Info
        html.AppendLine("<div class='invoice-info'>");
        html.AppendLine("<div class='invoice-details'>");
        html.AppendLine($"<h3>Invoice #{invoice.InvoiceNumber}</h3>");
        html.AppendLine($"<p><strong>Date:</strong> {invoice.IssuedDate:MMMM dd, yyyy}</p>");
        html.AppendLine($"<p><strong>Status:</strong> {invoice.Status}</p>");
        if (invoice.DueDate.HasValue)
            html.AppendLine($"<p><strong>Due Date:</strong> {invoice.DueDate.Value:MMMM dd, yyyy}</p>");
        html.AppendLine("</div>");

        if (!string.IsNullOrEmpty(invoice.CustomerName))
        {
            html.AppendLine("<div class='invoice-details'>");
            html.AppendLine("<h3>Bill To:</h3>");
            html.AppendLine($"<p><strong>{invoice.CustomerName}</strong></p>");
            if (!string.IsNullOrEmpty(invoice.CustomerEmail))
                html.AppendLine($"<p>{invoice.CustomerEmail}</p>");
            if (!string.IsNullOrEmpty(invoice.CustomerPhone))
                html.AppendLine($"<p>{invoice.CustomerPhone}</p>");
            if (!string.IsNullOrEmpty(invoice.BillingAddress))
                html.AppendLine($"<p>{invoice.BillingAddress}</p>");
            html.AppendLine("</div>");
        }
        html.AppendLine("</div>");

        // Line Items
        html.AppendLine("<div class='invoice-items'>");
        html.AppendLine("<table>");
        html.AppendLine("<thead>");
        html.AppendLine("<tr>");
        html.AppendLine("<th>Description</th>");
        html.AppendLine("<th class='text-right'>Qty</th>");
        html.AppendLine("<th class='text-right'>Unit Price</th>");
        html.AppendLine("<th class='text-right'>Tax</th>");
        html.AppendLine("<th class='text-right'>Total</th>");
        html.AppendLine("</tr>");
        html.AppendLine("</thead>");
        html.AppendLine("<tbody>");

        foreach (var item in invoice.InvoiceItems)
        {
            html.AppendLine("<tr>");
            html.AppendLine($"<td>{item.Description}</td>");
            html.AppendLine($"<td class='text-right'>{item.Quantity}</td>");
            html.AppendLine($"<td class='text-right'>{invoice.Currency} {item.UnitPrice:F2}</td>");
            html.AppendLine($"<td class='text-right'>{invoice.Currency} {item.TaxAmount:F2}</td>");
            html.AppendLine($"<td class='text-right'>{invoice.Currency} {item.TotalAmount:F2}</td>");
            html.AppendLine("</tr>");
        }

        html.AppendLine("</tbody>");
        html.AppendLine("</table>");
        html.AppendLine("</div>");

        // Totals
        html.AppendLine("<div class='totals'>");
        html.AppendLine("<table>");
        html.AppendLine($"<tr><td>Subtotal:</td><td class='text-right'>{invoice.Currency} {invoice.SubtotalAmount:F2}</td></tr>");
        html.AppendLine($"<tr><td>Tax ({invoice.TaxRate:P0}):</td><td class='text-right'>{invoice.Currency} {invoice.TaxAmount:F2}</td></tr>");

        if (invoice.DiscountAmount > 0)
        {
            var discountLabel = !string.IsNullOrEmpty(invoice.DiscountReason)
                ? $"Discount ({invoice.DiscountReason}):"
                : "Discount:";
            html.AppendLine($"<tr><td>{discountLabel}</td><td class='text-right'>-{invoice.Currency} {invoice.DiscountAmount:F2}</td></tr>");
        }

        if (invoice.TipAmount > 0)
            html.AppendLine($"<tr><td>Tip:</td><td class='text-right'>{invoice.Currency} {invoice.TipAmount:F2}</td></tr>");

        html.AppendLine($"<tr class='grand-total'><td>Total:</td><td class='text-right'>{invoice.Currency} {invoice.TotalAmount:F2}</td></tr>");
        html.AppendLine("</table>");
        html.AppendLine("</div>");

        // Notes
        if (!string.IsNullOrEmpty(invoice.Notes))
        {
            html.AppendLine("<div style='clear:both; margin-top: 30px;'>");
            html.AppendLine("<h3>Notes:</h3>");
            html.AppendLine($"<p>{invoice.Notes}</p>");
            html.AppendLine("</div>");
        }

        // Footer
        html.AppendLine("<div class='footer'>");
        html.AppendLine("<p>Thank you for your business!</p>");
        html.AppendLine($"<p>Generated on {DateTime.UtcNow:MMMM dd, yyyy HH:mm} UTC</p>");
        html.AppendLine("</div>");

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }
}
