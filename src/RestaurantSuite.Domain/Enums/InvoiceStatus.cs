namespace RestaurantSuite.Domain.Enums;

public enum InvoiceStatus
{
    Draft = 0,          // Being prepared
    Issued = 1,         // Sent/given to customer
    Paid = 2,           // Payment received in full
    PartiallyPaid = 3,  // Partial payment received
    Cancelled = 4,      // Cancelled before payment
    Refunded = 5,       // Refunded after payment
    Overdue = 6         // Past due date without payment
}
