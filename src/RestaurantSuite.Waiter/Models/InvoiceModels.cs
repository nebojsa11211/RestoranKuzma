namespace RestaurantSuite.Waiter.Models;

public enum InvoiceStatus
{
    Draft = 0,
    Issued = 1,
    Paid = 2,
    PartiallyPaid = 3,
    Cancelled = 4,
    Refunded = 5,
    Overdue = 6
}

public class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid? PaymentId { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal TaxRate { get; set; }
    public string? Notes { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? BillingAddress { get; set; }
    public string? DiscountReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<InvoiceItem> InvoiceItems { get; set; } = new();
}

public class InvoiceItem
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid? OrderItemId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CreateInvoiceRequest
{
    public Guid OrderId { get; set; }
    public decimal TaxRate { get; set; } = 0.08m;
    public string Currency { get; set; } = "USD";
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? BillingAddress { get; set; }
    public string? Notes { get; set; }
    public decimal TipAmount { get; set; }
    public bool AutoIssue { get; set; } = true;
}

public class InvoiceResponse
{
    public bool Success { get; set; }
    public Invoice? Invoice { get; set; }
    public string? ErrorMessage { get; set; }
}

public class InvoicesResponse
{
    public bool Success { get; set; }
    public List<Invoice> Invoices { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
