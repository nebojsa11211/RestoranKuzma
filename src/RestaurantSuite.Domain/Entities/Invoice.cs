using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Domain.Entities;

public class Invoice
{
    public Guid Id { get; private set; }
    public string InvoiceNumber { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid? PaymentId { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime IssuedDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public decimal SubtotalAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TipAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; }
    public decimal TaxRate { get; private set; }
    public string? Notes { get; private set; }
    public string? CustomerName { get; private set; }
    public string? CustomerEmail { get; private set; }
    public string? CustomerPhone { get; private set; }
    public string? BillingAddress { get; private set; }
    public string? DiscountReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public Order Order { get; private set; }
    public Payment? Payment { get; private set; }

    private readonly List<InvoiceItem> _invoiceItems = new();
    public IReadOnlyCollection<InvoiceItem> InvoiceItems => _invoiceItems.AsReadOnly();

    private Invoice() { }

    public static Invoice Create(
        string invoiceNumber,
        Guid orderId,
        decimal taxRate,
        string currency = "USD",
        string? customerName = null,
        string? customerEmail = null,
        string? customerPhone = null,
        string? billingAddress = null,
        string? notes = null,
        DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException("Invoice number cannot be empty", nameof(invoiceNumber));

        if (taxRate < 0 || taxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        return new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = invoiceNumber,
            OrderId = orderId,
            Status = InvoiceStatus.Draft,
            IssuedDate = DateTime.UtcNow,
            DueDate = dueDate,
            SubtotalAmount = 0,
            TaxAmount = 0,
            DiscountAmount = 0,
            TipAmount = 0,
            TotalAmount = 0,
            Currency = currency,
            TaxRate = taxRate,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            BillingAddress = billingAddress,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public InvoiceItem AddItem(
        string description,
        int quantity,
        decimal unitPrice,
        Guid? orderItemId = null)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Can only add items to draft invoices");

        var item = InvoiceItem.Create(Id, description, quantity, unitPrice, TaxRate, orderItemId);
        _invoiceItems.Add(item);
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
        return item;
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Can only remove items from draft invoices");

        var item = _invoiceItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _invoiceItems.Remove(item);
            RecalculateTotals();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ApplyDiscount(decimal amount, string reason)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Can only apply discounts to draft invoices");

        if (amount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(amount));

        if (amount > SubtotalAmount)
            throw new ArgumentException("Discount cannot exceed subtotal", nameof(amount));

        DiscountAmount = amount;
        DiscountReason = reason;
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTip(decimal tipAmount)
    {
        if (tipAmount < 0)
            throw new ArgumentException("Tip amount cannot be negative", nameof(tipAmount));

        TipAmount = tipAmount;
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCustomerDetails(
        string? customerName,
        string? customerEmail,
        string? customerPhone,
        string? billingAddress)
    {
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        CustomerPhone = customerPhone;
        BillingAddress = billingAddress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    private void RecalculateTotals()
    {
        SubtotalAmount = _invoiceItems.Sum(i => i.Subtotal);
        TaxAmount = _invoiceItems.Sum(i => i.TaxAmount);
        TotalAmount = SubtotalAmount + TaxAmount - DiscountAmount + TipAmount;
    }

    public void MarkAsIssued()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be issued");

        if (!_invoiceItems.Any())
            throw new InvalidOperationException("Cannot issue invoice with no items");

        Status = InvoiceStatus.Issued;
        IssuedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsPaid(Guid paymentId)
    {
        if (Status != InvoiceStatus.Issued && Status != InvoiceStatus.PartiallyPaid && Status != InvoiceStatus.Overdue)
            throw new InvalidOperationException("Only issued, partially paid, or overdue invoices can be marked as paid");

        Status = InvoiceStatus.Paid;
        PaymentId = paymentId;
        PaidDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsPartiallyPaid()
    {
        if (Status != InvoiceStatus.Issued && Status != InvoiceStatus.Overdue)
            throw new InvalidOperationException("Only issued or overdue invoices can be marked as partially paid");

        Status = InvoiceStatus.PartiallyPaid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsOverdue()
    {
        if (Status != InvoiceStatus.Issued && Status != InvoiceStatus.PartiallyPaid)
            throw new InvalidOperationException("Only issued or partially paid invoices can be marked as overdue");

        Status = InvoiceStatus.Overdue;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Refunded)
            throw new InvalidOperationException("Cannot cancel paid or refunded invoices");

        Status = InvoiceStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Refund()
    {
        if (Status != InvoiceStatus.Paid)
            throw new InvalidOperationException("Only paid invoices can be refunded");

        Status = InvoiceStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }
}
