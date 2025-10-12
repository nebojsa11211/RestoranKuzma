namespace RestaurantSuite.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }
    public string? SpecialInstructions { get; private set; }

    // Navigation properties
    public Order Order { get; private set; }
    public MenuItem MenuItem { get; private set; }

    private OrderItem() { }

    public static OrderItem Create(Guid orderId, Guid menuItemId, int quantity, decimal unitPrice, string? specialInstructions)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            MenuItemId = menuItemId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            SpecialInstructions = specialInstructions
        };

        orderItem.RecalculateSubtotal();
        return orderItem;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
        RecalculateSubtotal();
    }

    private void RecalculateSubtotal()
    {
        Subtotal = Quantity * UnitPrice;
    }
}
