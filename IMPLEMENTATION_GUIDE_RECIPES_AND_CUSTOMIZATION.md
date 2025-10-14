# Implementation Guide: Menu Item Recipes, Customization & Preparation Time

## ✅ COMPLETED WORK (Phases 1-2)

### Phase 1: Domain & Data Layer ✅
- ✅ Created `CustomizationType` enum (More/Less)
- ✅ Created `MenuItemIngredient` entity
- ✅ Created `OrderItemCustomization` entity
- ✅ Updated `MenuItem` with `PreparationTimeMinutes` and `Ingredients` collection
- ✅ Updated `OrderItem` with `Customizations` collection
- ✅ Updated `ApplicationDbContext` with new entity configurations
- ✅ Created and applied migration: `AddMenuItemIngredientsAndCustomizations`

### Phase 2: DTOs & Basic API ✅
- ✅ Created 7 new DTOs for ingredients and customizations
- ✅ Updated existing DTOs (`MenuItemDto`, `OrderItemDto`)
- ✅ Added preparation time endpoint to `MenuItemsController`
- ✅ Created `UpdateMenuItemPreparationTimeCommand` and handler

---

## 📋 REMAINING WORK (Phases 3-7)

### Phase 3: Complete API Layer

#### 3.1 Create MenuItemIngredientsController

**File**: `src/RestaurantSuite.Api/Controllers/MenuItemIngredientsController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Queries;

namespace RestaurantSuite.Api.Controllers;

/// <summary>
/// Menu item ingredients management endpoints (Admin only)
/// </summary>
[ApiController]
[Route("api/admin/menu/{menuItemId}/ingredients")]
[Authorize(Roles = "Admin")]
public class MenuItemIngredientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuItemIngredientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all ingredients for a menu item
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid menuItemId)
    {
        var query = new GetMenuItemIngredientsQuery { MenuItemId = menuItemId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Add an ingredient to a menu item
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(Guid menuItemId, [FromBody] CreateMenuItemIngredientDto dto)
    {
        try
        {
            var command = new AddMenuItemIngredientCommand
            {
                MenuItemId = menuItemId,
                IngredientName = dto.IngredientName,
                QuantityInGrams = dto.QuantityInGrams,
                IsMainIngredient = dto.IsMainIngredient,
                DisplayOrder = dto.DisplayOrder
            };

            var id = await _mediator.Send(command);
            return Ok(new { id });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an ingredient
    /// </summary>
    [HttpPut("{ingredientId}")]
    public async Task<IActionResult> Update(
        Guid menuItemId,
        Guid ingredientId,
        [FromBody] UpdateMenuItemIngredientDto dto)
    {
        try
        {
            var command = new UpdateMenuItemIngredientCommand
            {
                Id = ingredientId,
                MenuItemId = menuItemId,
                IngredientName = dto.IngredientName,
                QuantityInGrams = dto.QuantityInGrams,
                IsMainIngredient = dto.IsMainIngredient,
                DisplayOrder = dto.DisplayOrder
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete an ingredient
    /// </summary>
    [HttpDelete("{ingredientId}")]
    public async Task<IActionResult> Delete(Guid menuItemId, Guid ingredientId)
    {
        try
        {
            var command = new DeleteMenuItemIngredientCommand
            {
                MenuItemId = menuItemId,
                IngredientId = ingredientId
            };

            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
```

#### 3.2 Create MediatR Commands for Ingredients

**AddMenuItemIngredientCommand.cs**:
```csharp
using MediatR;

namespace RestaurantSuite.Application.Commands;

public class AddMenuItemIngredientCommand : IRequest<Guid>
{
    public Guid MenuItemId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal QuantityInGrams { get; set; }
    public bool IsMainIngredient { get; set; }
    public int DisplayOrder { get; set; }
}
```

**AddMenuItemIngredientCommandHandler.cs**:
```csharp
using MediatR;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Application.Commands;

public class AddMenuItemIngredientCommandHandler : IRequestHandler<AddMenuItemIngredientCommand, Guid>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddMenuItemIngredientCommandHandler(
        IMenuItemRepository menuItemRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddMenuItemIngredientCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(request.MenuItemId, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.MenuItemId} not found");

        menuItem.AddIngredient(
            request.IngredientName,
            request.QuantityInGrams,
            request.IsMainIngredient,
            request.DisplayOrder);

        _menuItemRepository.Update(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var addedIngredient = menuItem.Ingredients
            .OrderByDescending(i => i.CreatedAt)
            .First();

        return addedIngredient.Id;
    }
}
```

**UpdateMenuItemIngredientCommand.cs** & Handler:
```csharp
using MediatR;

namespace RestaurantSuite.Application.Commands;

public class UpdateMenuItemIngredientCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal QuantityInGrams { get; set; }
    public bool IsMainIngredient { get; set; }
    public int DisplayOrder { get; set; }
}
```

```csharp
using MediatR;
using RestaurantSuite.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantSuite.Application.Commands;

public class UpdateMenuItemIngredientCommandHandler : IRequestHandler<UpdateMenuItemIngredientCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMenuItemIngredientCommandHandler(
        IMenuItemRepository menuItemRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateMenuItemIngredientCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdWithIngredientsAsync(request.MenuItemId, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.MenuItemId} not found");

        var ingredient = menuItem.Ingredients.FirstOrDefault(i => i.Id == request.Id);
        if (ingredient == null)
            throw new KeyNotFoundException($"Ingredient with ID {request.Id} not found");

        ingredient.Update(
            request.IngredientName,
            request.QuantityInGrams,
            request.IsMainIngredient,
            request.DisplayOrder);

        _menuItemRepository.Update(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
```

**DeleteMenuItemIngredientCommand.cs** & Handler:
```csharp
using MediatR;

namespace RestaurantSuite.Application.Commands;

public class DeleteMenuItemIngredientCommand : IRequest
{
    public Guid MenuItemId { get; set; }
    public Guid IngredientId { get; set; }
}
```

```csharp
using MediatR;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Commands;

public class DeleteMenuItemIngredientCommandHandler : IRequestHandler<DeleteMenuItemIngredientCommand, Unit>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMenuItemIngredientCommandHandler(
        IMenuItemRepository menuItemRepository,
        IUnitOfWork unitOfWork)
    {
        _menuItemRepository = menuItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteMenuItemIngredientCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdWithIngredientsAsync(request.MenuItemId, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.MenuItemId} not found");

        menuItem.RemoveIngredient(request.IngredientId);

        _menuItemRepository.Update(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
```

#### 3.3 Create Query for Getting Ingredients

**GetMenuItemIngredientsQuery.cs**:
```csharp
using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetMenuItemIngredientsQuery : IRequest<List<MenuItemIngredientDto>>
{
    public Guid MenuItemId { get; set; }
}
```

**GetMenuItemIngredientsQueryHandler.cs**:
```csharp
using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetMenuItemIngredientsQueryHandler : IRequestHandler<GetMenuItemIngredientsQuery, List<MenuItemIngredientDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetMenuItemIngredientsQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<List<MenuItemIngredientDto>> Handle(GetMenuItemIngredientsQuery request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuItemRepository.GetByIdWithIngredientsAsync(request.MenuItemId, cancellationToken);
        if (menuItem == null)
            throw new KeyNotFoundException($"Menu item with ID {request.MenuItemId} not found");

        return menuItem.Ingredients
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new MenuItemIngredientDto
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                IngredientName = i.IngredientName,
                QuantityInGrams = i.QuantityInGrams,
                IsMainIngredient = i.IsMainIngredient,
                DisplayOrder = i.DisplayOrder,
                CreatedAt = i.CreatedAt
            })
            .ToList();
    }
}
```

#### 3.4 Update IMenuItemRepository

**File**: `src/RestaurantSuite.Application/Interfaces/IMenuItemRepository.cs`

Add these methods:
```csharp
Task<MenuItem?> GetByIdWithIngredientsAsync(Guid id, CancellationToken cancellationToken = default);
Task<List<MenuItem>> GetAllWithIngredientsAsync(bool? availableOnly = null, CancellationToken cancellationToken = default);
```

#### 3.5 Implement Repository Methods

**File**: Update your repository implementation (likely in Infrastructure.EF)

```csharp
public async Task<MenuItem?> GetByIdWithIngredientsAsync(Guid id, CancellationToken cancellationToken = default)
{
    return await _context.MenuItems
        .Include(m => m.Ingredients.OrderBy(i => i.DisplayOrder))
        .Include(m => m.Category)
        .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
}

public async Task<List<MenuItem>> GetAllWithIngredientsAsync(bool? availableOnly = null, CancellationToken cancellationToken = default)
{
    var query = _context.MenuItems
        .Include(m => m.Ingredients.OrderBy(i => i.DisplayOrder))
        .Include(m => m.Category)
        .AsQueryable();

    if (availableOnly == true)
        query = query.Where(m => m.IsAvailable);

    return await query.ToListAsync(cancellationToken);
}
```

#### 3.6 Update OrdersController for Customizations

**Update CreateOrderDto** to include customizations:
```csharp
public class CreateOrderItemDto
{
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
    public List<CreateOrderItemCustomizationDto> Customizations { get; set; } = new();
}
```

**Update CreateOrderCommand** to process customizations in the handler:
```csharp
// In CreateOrderCommandHandler, after adding order items:
foreach (var itemDto in request.OrderItems)
{
    order.AddItem(itemDto.MenuItemId, itemDto.Quantity, unitPrice, itemDto.SpecialInstructions);

    var orderItem = order.OrderItems.Last(); // Get the just-added item

    foreach (var customization in itemDto.Customizations)
    {
        orderItem.AddCustomization(
            customization.IngredientName,
            customization.CustomizationType,
            customization.Notes);
    }
}
```

#### 3.7 Add Role-Based Menu Endpoints

**Add to MenuItemsController**:
```csharp
/// <summary>
/// Get menu with full recipe details (Chef/Admin only)
/// </summary>
[HttpGet("chef/menu-with-recipes")]
[Authorize(Roles = "Chef,Admin")]
public async Task<IActionResult> GetMenuWithRecipes([FromQuery] Guid? categoryId)
{
    var query = new GetMenuWithRecipesQuery { CategoryId = categoryId };
    var result = await _mediator.Send(query);
    return Ok(result);
}

/// <summary>
/// Get menu with simplified ingredients (Guest view)
/// </summary>
[HttpGet("guest/menu")]
public async Task<IActionResult> GetGuestMenu([FromQuery] Guid? categoryId)
{
    var query = new GetGuestMenuQuery { CategoryId = categoryId };
    var result = await _mediator.Send(query);
    return Ok(result);
}
```

**Create GetMenuWithRecipesQuery**:
```csharp
using MediatR;
using RestaurantSuite.Application.DTOs;

namespace RestaurantSuite.Application.Queries;

public class GetMenuWithRecipesQuery : IRequest<List<MenuItemWithIngredientsDto>>
{
    public Guid? CategoryId { get; set; }
}
```

**GetMenuWithRecipesQueryHandler**:
```csharp
using MediatR;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Application.Queries;

public class GetMenuWithRecipesQueryHandler : IRequestHandler<GetMenuWithRecipesQuery, List<MenuItemWithIngredientsDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;

    public GetMenuWithRecipesQueryHandler(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }

    public async Task<List<MenuItemWithIngredientsDto>> Handle(GetMenuWithRecipesQuery request, CancellationToken cancellationToken)
    {
        var menuItems = await _menuItemRepository.GetAllWithIngredientsAsync(true, cancellationToken);

        if (request.CategoryId.HasValue)
            menuItems = menuItems.Where(m => m.CategoryId == request.CategoryId.Value).ToList();

        return menuItems.Select(m => new MenuItemWithIngredientsDto
        {
            Id = m.Id,
            CategoryId = m.CategoryId,
            Name = m.Name,
            Description = m.Description,
            Price = m.Price,
            ImageUrl = m.ImageUrl,
            IsAvailable = m.IsAvailable,
            PreparationTimeMinutes = m.PreparationTimeMinutes,
            CategoryName = m.Category.Name,
            Ingredients = m.Ingredients.Select(i => new MenuItemIngredientDto
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                IngredientName = i.IngredientName,
                QuantityInGrams = i.QuantityInGrams,
                IsMainIngredient = i.IsMainIngredient,
                DisplayOrder = i.DisplayOrder,
                CreatedAt = i.CreatedAt
            }).ToList(),
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        }).ToList();
    }
}
```

**Create GetGuestMenuQuery** (similar pattern, but returns `GuestMenuItemDto`):
```csharp
public async Task<List<GuestMenuItemDto>> Handle(GetGuestMenuQuery request, CancellationToken cancellationToken)
{
    var menuItems = await _menuItemRepository.GetAllWithIngredientsAsync(true, cancellationToken);

    if (request.CategoryId.HasValue)
        menuItems = menuItems.Where(m => m.CategoryId == request.CategoryId.Value).ToList();

    return menuItems.Select(m => new GuestMenuItemDto
    {
        Id = m.Id,
        CategoryId = m.CategoryId,
        Name = m.Name,
        Description = m.Description,
        Price = m.Price,
        ImageUrl = m.ImageUrl,
        IsAvailable = m.IsAvailable,
        PreparationTimeMinutes = m.PreparationTimeMinutes,
        CategoryName = m.Category.Name,
        MainIngredients = m.Ingredients
            .Where(i => i.IsMainIngredient)
            .OrderBy(i => i.DisplayOrder)
            .Select(i => i.IngredientName)
            .ToList()
    }).ToList();
}
```

---

### Phase 4: Admin UI (Blazor Server)

#### 4.1 Create Recipe Management Page

**File**: `src/RestaurantSuite.Admin/Pages/RecipeManagement.razor`

```razor
@page "/recipes"
@using RestaurantSuite.Application.DTOs
@inject HttpClient Http
@inject NavigationManager Navigation

<PageTitle>Recipe Management</PageTitle>

<h3>Recipe Management</h3>

<div class="mb-3">
    <label>Select Menu Item:</label>
    <select class="form-select" @onchange="OnMenuItemSelected">
        <option value="">-- Select Menu Item --</option>
        @foreach (var item in menuItems)
        {
            <option value="@item.Id">@item.Name</option>
        }
    </select>
</div>

@if (selectedMenuItem != null)
{
    <div class="card mb-3">
        <div class="card-header">
            <h5>@selectedMenuItem.Name - Recipe Details</h5>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <label>Preparation Time (minutes):</label>
                <input type="number" class="form-control" @bind="preparationTime" />
                <button class="btn btn-primary mt-2" @onclick="UpdatePreparationTime">Update Prep Time</button>
            </div>

            <h6>Ingredients:</h6>
            <table class="table">
                <thead>
                    <tr>
                        <th>Ingredient</th>
                        <th>Quantity (g)</th>
                        <th>Main Ingredient</th>
                        <th>Display Order</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var ingredient in ingredients)
                    {
                        <tr>
                            <td>@ingredient.IngredientName</td>
                            <td>@ingredient.QuantityInGrams</td>
                            <td>@(ingredient.IsMainIngredient ? "Yes" : "No")</td>
                            <td>@ingredient.DisplayOrder</td>
                            <td>
                                <button class="btn btn-sm btn-danger" @onclick="() => DeleteIngredient(ingredient.Id)">Delete</button>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>

            <h6>Add New Ingredient:</h6>
            <div class="row">
                <div class="col">
                    <input type="text" class="form-control" placeholder="Ingredient Name" @bind="newIngredient.IngredientName" />
                </div>
                <div class="col">
                    <input type="number" class="form-control" placeholder="Quantity (g)" @bind="newIngredient.QuantityInGrams" />
                </div>
                <div class="col">
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" @bind="newIngredient.IsMainIngredient" />
                        <label class="form-check-label">Main Ingredient</label>
                    </div>
                </div>
                <div class="col">
                    <input type="number" class="form-control" placeholder="Order" @bind="newIngredient.DisplayOrder" />
                </div>
                <div class="col">
                    <button class="btn btn-success" @onclick="AddIngredient">Add</button>
                </div>
            </div>
        </div>
    </div>
}

@code {
    private List<MenuItemDto> menuItems = new();
    private MenuItemDto? selectedMenuItem;
    private List<MenuItemIngredientDto> ingredients = new();
    private CreateMenuItemIngredientDto newIngredient = new();
    private int? preparationTime;

    protected override async Task OnInitializedAsync()
    {
        await LoadMenuItems();
    }

    private async Task LoadMenuItems()
    {
        try
        {
            menuItems = await Http.GetFromJsonAsync<List<MenuItemDto>>("api/menu") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading menu items: {ex.Message}");
        }
    }

    private async Task OnMenuItemSelected(ChangeEventArgs e)
    {
        var menuItemId = e.Value?.ToString();
        if (string.IsNullOrEmpty(menuItemId)) return;

        selectedMenuItem = menuItems.FirstOrDefault(m => m.Id.ToString() == menuItemId);
        if (selectedMenuItem != null)
        {
            preparationTime = selectedMenuItem.PreparationTimeMinutes;
            await LoadIngredients(Guid.Parse(menuItemId));
        }
    }

    private async Task LoadIngredients(Guid menuItemId)
    {
        try
        {
            ingredients = await Http.GetFromJsonAsync<List<MenuItemIngredientDto>>($"api/admin/menu/{menuItemId}/ingredients") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading ingredients: {ex.Message}");
        }
    }

    private async Task UpdatePreparationTime()
    {
        if (selectedMenuItem == null) return;

        try
        {
            var response = await Http.PatchAsJsonAsync($"api/admin/menu/{selectedMenuItem.Id}/preparation-time", preparationTime);
            if (response.IsSuccessStatusCode)
            {
                // Success feedback
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating prep time: {ex.Message}");
        }
    }

    private async Task AddIngredient()
    {
        if (selectedMenuItem == null) return;

        try
        {
            var response = await Http.PostAsJsonAsync($"api/admin/menu/{selectedMenuItem.Id}/ingredients", newIngredient);
            if (response.IsSuccessStatusCode)
            {
                await LoadIngredients(selectedMenuItem.Id);
                newIngredient = new CreateMenuItemIngredientDto();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding ingredient: {ex.Message}");
        }
    }

    private async Task DeleteIngredient(Guid ingredientId)
    {
        if (selectedMenuItem == null) return;

        try
        {
            var response = await Http.DeleteAsync($"api/admin/menu/{selectedMenuItem.Id}/ingredients/{ingredientId}");
            if (response.IsSuccessStatusCode)
            {
                await LoadIngredients(selectedMenuItem.Id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting ingredient: {ex.Message}");
        }
    }
}
```

---

### Phase 5: Chef UI (Blazor WASM)

#### 5.1 Update Order Display to Show Recipes and Customizations

**File**: Update `src/RestaurantSuite.Chef/Components/OrderCard.razor` or similar

```razor
<div class="order-card">
    <h5>Order #@Order.Id.ToString().Substring(0, 8)</h5>
    <p>Table: @Order.TableNumber</p>

    @foreach (var item in Order.Items)
    {
        <div class="order-item">
            <h6>@item.MenuItemName (x@item.Quantity)</h6>

            @* Show recipe if available *@
            @if (item.Recipe != null && item.Recipe.Any())
            {
                <div class="recipe-details">
                    <strong>Recipe:</strong>
                    <ul>
                        @foreach (var ingredient in item.Recipe)
                        {
                            <li>@ingredient.IngredientName: @ingredient.QuantityInGrams g</li>
                        }
                    </ul>
                </div>
            }

            @* Show customizations prominently *@
            @if (item.Customizations != null && item.Customizations.Any())
            {
                <div class="customizations alert alert-warning">
                    <strong>⚠️ CUSTOMIZATIONS:</strong>
                    <ul>
                        @foreach (var custom in item.Customizations)
                        {
                            <li>
                                @custom.IngredientName:
                                @(custom.CustomizationType == CustomizationType.More ? "EXTRA" : "LESS")
                                @if (!string.IsNullOrEmpty(custom.Notes))
                                {
                                    <span> - @custom.Notes</span>
                                }
                            </li>
                        }
                    </ul>
                </div>
            }

            @if (!string.IsNullOrEmpty(item.SpecialInstructions))
            {
                <p class="special-instructions"><em>@item.SpecialInstructions</em></p>
            }
        </div>
    }
</div>
```

---

### Phase 6: Guest UI (Blazor WASM)

#### 6.1 Update Menu Display with Ingredients and Prep Time

**File**: `src/RestaurantSuite.Guest/Pages/Menu.razor`

```razor
@page "/menu"
@inject HttpClient Http

<h3>Menu</h3>

@foreach (var item in menuItems)
{
    <div class="card mb-3">
        <div class="card-body">
            <h5>@item.Name - $@item.Price</h5>
            <p>@item.Description</p>

            @if (item.PreparationTimeMinutes.HasValue)
            {
                <p class="text-muted">
                    <small>⏱️ Prep time: ~@item.PreparationTimeMinutes minutes</small>
                </p>
            }

            @if (item.MainIngredients.Any())
            {
                <p>
                    <strong>Ingredients:</strong> @string.Join(", ", item.MainIngredients)
                </p>
            }

            <button class="btn btn-primary" @onclick="() => ShowCustomization(item)">
                Add to Order
            </button>
        </div>
    </div>
}

@if (showCustomizationModal && selectedItem != null)
{
    <div class="modal show d-block" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Customize: @selectedItem.Name</h5>
                    <button type="button" class="btn-close" @onclick="CloseCustomization"></button>
                </div>
                <div class="modal-body">
                    <p>Customize your ingredients:</p>

                    @foreach (var ingredient in selectedItem.MainIngredients)
                    {
                        <div class="mb-2">
                            <label>@ingredient:</label>
                            <div class="btn-group">
                                <button class="btn btn-outline-secondary"
                                        @onclick="() => AddCustomization(ingredient, CustomizationType.Less)">
                                    Less
                                </button>
                                <button class="btn btn-outline-primary"
                                        @onclick="() => AddCustomization(ingredient, CustomizationType.More)">
                                    Extra
                                </button>
                            </div>
                        </div>
                    }

                    <div class="mt-3">
                        <label>Quantity:</label>
                        <input type="number" class="form-control" @bind="quantity" min="1" />
                    </div>

                    <div class="mt-3">
                        <label>Special Instructions:</label>
                        <textarea class="form-control" @bind="specialInstructions"></textarea>
                    </div>

                    @if (currentCustomizations.Any())
                    {
                        <div class="mt-3">
                            <strong>Your customizations:</strong>
                            <ul>
                                @foreach (var custom in currentCustomizations)
                                {
                                    <li>
                                        @custom.IngredientName: @(custom.CustomizationType == CustomizationType.More ? "Extra" : "Less")
                                        <button class="btn btn-sm btn-danger" @onclick="() => RemoveCustomization(custom)">×</button>
                                    </li>
                                }
                            </ul>
                        </div>
                    }
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @onclick="CloseCustomization">Cancel</button>
                    <button type="button" class="btn btn-primary" @onclick="AddToOrder">Add to Order</button>
                </div>
            </div>
        </div>
    </div>
}

@code {
    private List<GuestMenuItemDto> menuItems = new();
    private GuestMenuItemDto? selectedItem;
    private bool showCustomizationModal = false;
    private List<CreateOrderItemCustomizationDto> currentCustomizations = new();
    private int quantity = 1;
    private string specialInstructions = "";

    protected override async Task OnInitializedAsync()
    {
        menuItems = await Http.GetFromJsonAsync<List<GuestMenuItemDto>>("api/guest/menu") ?? new();
    }

    private void ShowCustomization(GuestMenuItemDto item)
    {
        selectedItem = item;
        currentCustomizations.Clear();
        quantity = 1;
        specialInstructions = "";
        showCustomizationModal = true;
    }

    private void CloseCustomization()
    {
        showCustomizationModal = false;
        selectedItem = null;
    }

    private void AddCustomization(string ingredient, CustomizationType type)
    {
        // Remove any existing customization for this ingredient
        currentCustomizations.RemoveAll(c => c.IngredientName == ingredient);

        currentCustomizations.Add(new CreateOrderItemCustomizationDto
        {
            IngredientName = ingredient,
            CustomizationType = type
        });
    }

    private void RemoveCustomization(CreateOrderItemCustomizationDto custom)
    {
        currentCustomizations.Remove(custom);
    }

    private async Task AddToOrder()
    {
        // Add to cart logic here with customizations
        // currentCustomizations contains the selected customizations
        CloseCustomization();
    }
}
```

---

### Phase 7: Testing & Data Seeding

#### 7.1 Update DatabaseInitializer with Sample Recipes

**File**: Update `DatabaseInitializer/Program.cs`

```csharp
// After creating menu items, add ingredients
var pizzaId = pizzaMenuItem.Id;

var pizzaIngredients = new[]
{
    MenuItemIngredient.Create(pizzaId, "Mozzarella Cheese", 150m, true, 1),
    MenuItemIngredient.Create(pizzaId, "Tomato Sauce", 100m, true, 2),
    MenuItemIngredient.Create(pizzaId, "Pizza Dough", 250m, true, 3),
    MenuItemIngredient.Create(pizzaId, "Olive Oil", 15m, false, 4),
    MenuItemIngredient.Create(pizzaId, "Basil", 5m, false, 5)
};

foreach (var ingredient in pizzaIngredients)
{
    pizzaMenuItem.AddIngredient(
        ingredient.IngredientName,
        ingredient.QuantityInGrams,
        ingredient.IsMainIngredient,
        ingredient.DisplayOrder);
}

pizzaMenuItem.SetPreparationTime(15);

await context.SaveChangesAsync();
```

---

## 🔧 Additional Configuration

### Update appsettings.json (if needed)

No specific configuration changes needed for this feature.

### Register Services (if creating custom services)

In `Program.cs`:
```csharp
// If you created any custom services, register them here
builder.Services.AddScoped<IIngredientService, IngredientService>();
```

---

## ✅ Testing Checklist

### API Testing
- [ ] Test preparation time update endpoint
- [ ] Test ingredient CRUD operations
- [ ] Test creating orders with customizations
- [ ] Test role-based menu endpoints (Chef vs Guest)

### UI Testing (Admin)
- [ ] Can add ingredients to menu items
- [ ] Can update ingredient quantities
- [ ] Can mark ingredients as main/secondary
- [ ] Can set preparation time

### UI Testing (Chef)
- [ ] Recipes display correctly with quantities
- [ ] Customizations are prominently highlighted
- [ ] Preparation times visible

### UI Testing (Guest)
- [ ] Only main ingredients shown (no quantities)
- [ ] Preparation time displayed
- [ ] Can customize ingredients (more/less)
- [ ] Customizations saved to order

---

## 📊 Database Verification

Run these queries to verify data:

```sql
-- Check MenuItems with preparation time
SELECT Id, Name, PreparationTimeMinutes FROM MenuItems;

-- Check Ingredients
SELECT mi.Name, mii.IngredientName, mii.QuantityInGrams, mii.IsMainIngredient
FROM MenuItems mi
JOIN MenuItemIngredients mii ON mi.Id = mii.MenuItemId
ORDER BY mi.Name, mii.DisplayOrder;

-- Check Order Customizations
SELECT o.Id, oi.MenuItemId, oic.IngredientName, oic.CustomizationType
FROM Orders o
JOIN OrderItems oi ON o.Id = oi.OrderId
JOIN OrderItemCustomizations oic ON oi.Id = oic.OrderItemId;
```

---

## 🎯 Summary

**Completed:**
- ✅ Full domain model with entities
- ✅ Database schema and migration
- ✅ DTOs for all operations
- ✅ Preparation time API endpoint

**To Complete:**
1. Ingredient management commands & handlers
2. MenuItemIngredientsController
3. Update repositories with Include methods
4. Update OrdersController for customizations
5. Role-based menu query endpoints
6. Admin UI for recipe management
7. Chef UI updates for recipes & customizations
8. Guest UI for viewing ingredients & customizing
9. Database seeding with sample recipes
10. End-to-end testing

**Estimated remaining time:** 4-6 hours of development work

---

This guide provides all the code templates and patterns you need to complete the implementation. Follow the phases in order, test each component as you build it, and you'll have a fully functional recipe and customization system!
