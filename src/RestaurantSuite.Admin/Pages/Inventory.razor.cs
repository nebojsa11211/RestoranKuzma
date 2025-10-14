using Microsoft.AspNetCore.Components;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Admin.Pages;

public partial class Inventory
{
    private List<InventoryItemDto> inventoryItems = new();
    private IEnumerable<InventoryItemDto> filteredItems => inventoryItems
        .Where(i => (string.IsNullOrEmpty(searchTerm) ||
                    i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    i.SKU.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                    (!showOnlyLowStock || i.IsLowStock));

    private bool isLoading = true;
    private string errorMessage = string.Empty;
    private string searchTerm = string.Empty;
    private bool showOnlyLowStock = false;

    private int lowStockCount => inventoryItems.Count(i => i.IsLowStock);
    private decimal totalValue => inventoryItems.Sum(i => i.CurrentStock * i.UnitCost);

    // Modals
    private bool showAddModal = false;
    private bool showEditModal = false;
    private bool showAdjustModal = false;
    private InventoryItemDto? selectedItem = null;
    private CreateInventoryItemCommand currentItem = new();

    // Adjustment
    private StockTransactionType adjustmentType = StockTransactionType.Purchase;
    private decimal adjustmentQuantity = 0;
    private string adjustmentNotes = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadInventory();
    }

    private async Task LoadInventory()
    {
        try
        {
            isLoading = true;
            errorMessage = string.Empty;
            inventoryItems = await ApiService.GetInventoryItemsAsync();
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading inventory: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ToggleLowStockFilter()
    {
        showOnlyLowStock = !showOnlyLowStock;
    }

    private void ShowAddItemModal()
    {
        currentItem = new CreateInventoryItemCommand
        {
            Unit = UnitOfMeasure.Kilograms,
            InitialStock = 0,
            MinimumStockLevel = 0,
            UnitCost = 0
        };
        showAddModal = true;
    }

    private void ShowEditItemModal(InventoryItemDto item)
    {
        selectedItem = item;
        currentItem = new CreateInventoryItemCommand
        {
            Name = item.Name,
            Description = item.Description,
            SKU = item.SKU,
            Unit = item.Unit,
            InitialStock = item.CurrentStock,
            MinimumStockLevel = item.MinimumStockLevel,
            UnitCost = item.UnitCost
        };
        showEditModal = true;
    }

    private void ShowAdjustStockModal(InventoryItemDto item)
    {
        selectedItem = item;
        adjustmentType = StockTransactionType.Purchase;
        adjustmentQuantity = 0;
        adjustmentNotes = string.Empty;
        showAdjustModal = true;
    }

    private void ShowTransactionHistory(InventoryItemDto item)
    {
        // Navigate to transaction history page or show modal
        // TODO: Implement transaction history view
    }

    private void CloseModals()
    {
        showAddModal = false;
        showEditModal = false;
        showAdjustModal = false;
        selectedItem = null;
    }

    private async Task SaveItem()
    {
        try
        {
            if (showAddModal)
            {
                await ApiService.CreateInventoryItemAsync(currentItem);
            }
            else if (showEditModal && selectedItem != null)
            {
                var updateCommand = new UpdateInventoryItemCommand
                {
                    Id = selectedItem.Id,
                    Name = currentItem.Name,
                    Description = currentItem.Description,
                    SKU = currentItem.SKU,
                    Unit = currentItem.Unit,
                    MinimumStockLevel = currentItem.MinimumStockLevel,
                    UnitCost = currentItem.UnitCost
                };
                await ApiService.UpdateInventoryItemAsync(selectedItem.Id, updateCommand);
            }

            CloseModals();
            await LoadInventory();
        }
        catch (Exception ex)
        {
            errorMessage = $"Error saving item: {ex.Message}";
        }
    }

    private async Task SaveAdjustment()
    {
        if (selectedItem == null) return;

        try
        {
            var command = new AdjustInventoryStockCommand
            {
                InventoryItemId = selectedItem.Id,
                Quantity = adjustmentQuantity,
                TransactionType = adjustmentType,
                Notes = adjustmentNotes,
                CreatedBy = Guid.Empty // TODO: Get from auth context
            };

            await ApiService.AdjustInventoryStockAsync(selectedItem.Id, command);

            CloseModals();
            await LoadInventory();
        }
        catch (Exception ex)
        {
            errorMessage = $"Error adjusting stock: {ex.Message}";
        }
    }
}
