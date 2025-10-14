namespace RestaurantSuite.Domain.Entities;

public class InvoiceItem
{
    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Guid? OrderItemId { get; private set; }
    public string Description { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }

    // Navigation properties
    public Invoice Invoice { get; private set; }
    public OrderItem? OrderItem { get; private set; }

    private InvoiceItem() { }

    public static InvoiceItem Create(
        Guid invoiceId,
        string description,
        int quantity,
        decimal unitPrice,
        decimal taxRate,
        Guid? orderItemId = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (taxRate < 0 || taxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

        var item = new InvoiceItem
        {
            Id = Guid.NewGuid(),
            InvoiceId = invoiceId,
            OrderItemId = orderItemId,
            Description = description,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        item.RecalculateAmounts(taxRate);
        return item;
    }

    public void UpdateQuantity(int newQuantity, decimal taxRate)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
        RecalculateAmounts(taxRate);
    }

    public void UpdateUnitPrice(decimal newPrice, decimal taxRate)
    {
        if (newPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(newPrice));

        UnitPrice = newPrice;
        RecalculateAmounts(taxRate);
    }

    public void RecalculateAmounts(decimal taxRate)
    {
        if (taxRate < 0 || taxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

        Subtotal = Quantity * UnitPrice;
        TaxAmount = Subtotal * taxRate;
        TotalAmount = Subtotal + TaxAmount;
    }
}
