namespace RestaurantSuite.Application.DTOs;

public class InvoiceItemDto
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
