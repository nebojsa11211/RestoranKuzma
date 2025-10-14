using MediatR;

namespace RestaurantSuite.Application.Commands;

public class CreateInvoiceFromOrderCommand : IRequest<Guid>
{
    public Guid OrderId { get; set; }
    public decimal TaxRate { get; set; } = 0.08m; // 8% default
    public string Currency { get; set; } = "USD";
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? BillingAddress { get; set; }
    public string? Notes { get; set; }
    public decimal TipAmount { get; set; }
    public bool AutoIssue { get; set; } = true;
}
