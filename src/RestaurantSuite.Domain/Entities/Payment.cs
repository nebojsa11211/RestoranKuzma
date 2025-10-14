using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public decimal TipAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? TransactionId { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public Order Order { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, PaymentMethod method, decimal amount, decimal tipAmount, string? notes = null)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (tipAmount < 0)
            throw new ArgumentException("Tip amount cannot be negative", nameof(tipAmount));

        return new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Method = method,
            Status = PaymentStatus.Pending,
            Amount = amount,
            TipAmount = tipAmount,
            TotalAmount = amount + tipAmount,
            CreatedAt = DateTime.UtcNow,
            Notes = notes
        };
    }

    public void MarkAsCompleted(string? transactionId = null)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be completed");

        Status = PaymentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        TransactionId = transactionId;
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be marked as failed");

        Status = PaymentStatus.Failed;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Only completed payments can be refunded");

        Status = PaymentStatus.Refunded;
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
    }
}
