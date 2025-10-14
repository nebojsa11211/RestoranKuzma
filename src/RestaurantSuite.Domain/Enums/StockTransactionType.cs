namespace RestaurantSuite.Domain.Enums;

public enum StockTransactionType
{
    Purchase = 0,       // Stock added from supplier purchase
    Adjustment = 1,     // Manual stock adjustment (positive or negative)
    Consumption = 2,    // Stock used for order preparation
    Wastage = 3,        // Stock wasted/spoiled
    Return = 4          // Stock returned to supplier
}
