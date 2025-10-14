using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Handler that automatically reduces inventory stock when an order is being prepared
/// This implements the core automation logic of the inventory tracking system
/// </summary>
public class ReduceStockForOrderCommandHandler : IRequestHandler<ReduceStockForOrderCommand, ReduceStockForOrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReduceStockForOrderCommandHandler(
        IOrderRepository orderRepository,
        IRecipeRepository recipeRepository,
        IInventoryItemRepository inventoryItemRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _recipeRepository = recipeRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReduceStockForOrderResult> Handle(ReduceStockForOrderCommand request, CancellationToken cancellationToken)
    {
        var result = new ReduceStockForOrderResult();

        // Get order with items
        var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            result.Success = false;
            result.Messages.Add($"Order {request.OrderId} not found");
            return result;
        }

        // Dictionary to accumulate ingredient requirements: InventoryItemId -> Total Quantity Needed
        var ingredientRequirements = new Dictionary<Guid, decimal>();
        var menuItemsWithoutRecipes = new List<string>();

        // Calculate total ingredient requirements for all items in the order
        foreach (var orderItem in order.OrderItems)
        {
            var recipe = await _recipeRepository.GetByMenuItemIdWithIngredientsAsync(orderItem.MenuItemId, cancellationToken);

            if (recipe == null || !recipe.IsActive)
            {
                // No recipe defined for this menu item - log warning but continue
                menuItemsWithoutRecipes.Add($"{orderItem.MenuItem?.Name ?? "Unknown"} (x{orderItem.Quantity})");
                continue;
            }

            // For each ingredient in the recipe, multiply by order quantity
            foreach (var recipeIngredient in recipe.Ingredients)
            {
                var totalQuantityNeeded = recipeIngredient.QuantityRequired * orderItem.Quantity;

                if (ingredientRequirements.ContainsKey(recipeIngredient.InventoryItemId))
                {
                    ingredientRequirements[recipeIngredient.InventoryItemId] += totalQuantityNeeded;
                }
                else
                {
                    ingredientRequirements[recipeIngredient.InventoryItemId] = totalQuantityNeeded;
                }
            }
        }

        // Check for sufficient stock before making any changes
        var insufficientStockItems = new List<string>();
        var inventoryItems = new Dictionary<Guid, InventoryItem>();

        foreach (var kvp in ingredientRequirements)
        {
            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(kvp.Key, cancellationToken);
            if (inventoryItem == null)
            {
                insufficientStockItems.Add($"Inventory item {kvp.Key} not found");
                continue;
            }

            inventoryItems[kvp.Key] = inventoryItem;

            if (inventoryItem.CurrentStock < kvp.Value)
            {
                insufficientStockItems.Add(
                    $"{inventoryItem.Name}: Required {kvp.Value} {inventoryItem.Unit}, Available {inventoryItem.CurrentStock} {inventoryItem.Unit}");
            }
        }

        // If insufficient stock or missing items, fail without changes
        if (insufficientStockItems.Any())
        {
            result.Success = false;
            result.Messages.Add("Insufficient stock for the following items:");
            result.Messages.AddRange(insufficientStockItems);
            return result;
        }

        // All checks passed - now reduce stock and create transactions
        foreach (var kvp in ingredientRequirements)
        {
            var inventoryItem = inventoryItems[kvp.Key];
            var previousStock = inventoryItem.CurrentStock;
            var quantityToReduce = kvp.Value;

            // Reduce stock
            inventoryItem.ReduceStock(quantityToReduce);

            // Create stock transaction record
            var transaction = StockTransaction.Create(
                inventoryItem.Id,
                StockTransactionType.Consumption,
                -quantityToReduce,  // Negative for consumption
                previousStock,
                inventoryItem.CurrentStock,
                request.ProcessedBy,
                order.Id,
                $"Stock consumed for Order #{order.Id}");

            _inventoryItemRepository.Update(inventoryItem);
            await _stockTransactionRepository.AddAsync(transaction, cancellationToken);

            result.Reductions.Add(new StockReductionDetail
            {
                InventoryItemId = inventoryItem.Id,
                InventoryItemName = inventoryItem.Name,
                QuantityReduced = quantityToReduce,
                PreviousStock = previousStock,
                NewStock = inventoryItem.CurrentStock
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        result.Success = true;
        result.Messages.Add($"Successfully reduced stock for {result.Reductions.Count} ingredient(s)");

        if (menuItemsWithoutRecipes.Any())
        {
            result.Messages.Add($"Warning: No recipes defined for: {string.Join(", ", menuItemsWithoutRecipes)}");
        }

        return result;
    }
}
