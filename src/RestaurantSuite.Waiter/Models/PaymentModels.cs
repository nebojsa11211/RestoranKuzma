namespace RestaurantSuite.Waiter.Models;

public enum PaymentMethod
{
    Cash = 0,
    Card = 1,
    CreditCard = 2,
    DebitCard = 3,
    Terminal = 4,
    MobilePayment = 5,
    GiftCard = 6
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Additional payment details
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
}

public class CreatePaymentRequest
{
    public Guid OrderId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public decimal TipAmount { get; set; }
    public string? Notes { get; set; }
}

public class SplitPaymentRequest
{
    public Guid OrderId { get; set; }
    public List<SplitPaymentItem> SplitItems { get; set; } = new();
}

public class SplitPaymentItem
{
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public decimal TipAmount { get; set; }
    public string? Notes { get; set; }
}

public class PaymentResponse
{
    public bool Success { get; set; }
    public Payment? Payment { get; set; }
    public string? ErrorMessage { get; set; }
}

public class PaymentListResponse
{
    public bool Success { get; set; }
    public List<Payment> Payments { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
