using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.DTOs;

/// <summary>
/// Data transfer object for inventory item information
/// </summary>
public class InventoryItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal MinimumStockLevel { get; set; }
    public decimal UnitCost { get; set; }
    public bool IsActive { get; set; }
    public bool IsLowStock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
