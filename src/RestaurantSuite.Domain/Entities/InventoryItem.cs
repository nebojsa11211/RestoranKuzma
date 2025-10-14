using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    public decimal CurrentStock { get; private set; }
    public UnitOfMeasure Unit { get; private set; }
    public decimal MinimumStockLevel { get; private set; }
    public decimal UnitCost { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private InventoryItem() { }

    public static InventoryItem Create(
        string name,
        string description,
        string sku,
        decimal initialStock,
        UnitOfMeasure unit,
        decimal minimumStockLevel,
        decimal unitCost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be empty", nameof(sku));

        if (initialStock < 0)
            throw new ArgumentException("Initial stock cannot be negative", nameof(initialStock));

        if (minimumStockLevel < 0)
            throw new ArgumentException("Minimum stock level cannot be negative", nameof(minimumStockLevel));

        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative", nameof(unitCost));

        return new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            SKU = sku.Trim().ToUpperInvariant(),
            CurrentStock = initialStock,
            Unit = unit,
            MinimumStockLevel = minimumStockLevel,
            UnitCost = unitCost,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string name, string description, string sku, UnitOfMeasure unit, decimal minimumStockLevel, decimal unitCost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be empty", nameof(sku));

        if (minimumStockLevel < 0)
            throw new ArgumentException("Minimum stock level cannot be negative", nameof(minimumStockLevel));

        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative", nameof(unitCost));

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        SKU = sku.Trim().ToUpperInvariant();
        Unit = unit;
        MinimumStockLevel = minimumStockLevel;
        UnitCost = unitCost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdjustStock(decimal quantity)
    {
        var newStock = CurrentStock + quantity;

        if (newStock < 0)
            throw new InvalidOperationException($"Insufficient stock. Current: {CurrentStock}, Requested: {Math.Abs(quantity)}");

        CurrentStock = newStock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReduceStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (CurrentStock < quantity)
            throw new InvalidOperationException($"Insufficient stock. Current: {CurrentStock}, Requested: {quantity}");

        CurrentStock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        CurrentStock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStock(decimal quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

        CurrentStock = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLowStock()
    {
        return CurrentStock <= MinimumStockLevel;
    }
}
