using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.DTOs;

public class InvoiceDto
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
    public List<InvoiceItemDto> InvoiceItems { get; set; } = new();
}
