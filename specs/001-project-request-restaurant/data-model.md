# Data Model Specification

**Feature**: Restaurant Management & Ordering Suite
**Branch**: 001-project-request-restaurant
**Date**: 2025-10-06

## Overview
This document defines the complete entity model for the restaurant management system, including relationships, validation rules, and state transitions.

---

## Entity Relationship Diagram (Conceptual)

```
Restaurant (singleton)

MenuCategory ────── (*) MenuItem ────── (*) MenuItemOption ────── (*) MenuItemModifier
                                 │
InventoryItem                    └─── (*) Recipe (join)

Table

Reservation ────── (0..1) Table

Order ────── (*) OrderLine ────── (1) MenuItem
  │
  └─── (*) Payment

KitchenStation

Printer

AuditLog

User (1) ────── (0..1) Staff
     (1) ────── (0..1) LoyaltyAccount
```

---

## Core Entities

### Restaurant

**Purpose**: Represents the restaurant 'Restoran Kuzma' configuration (singleton).

**Fields**:
```csharp
public class Restaurant
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // Required, max 200 chars
    public string Address { get; set; }           // Required, max 500 chars
    public string Timezone { get; set; }          // IANA timezone (e.g., "America/New_York")
    public string Currency { get; set; }          // ISO 4217 code (e.g., "USD")
    public string SettingsJson { get; set; }      // JSON for flexible config
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Validation** (FR-011):
- Name: Required, 3-200 characters
- Address: Required
- Timezone: Valid IANA timezone
- Currency: Valid ISO 4217 code

**Indexes**:
- Primary: Id (clustered)

---

### User

**Purpose**: Represents any person interacting with the system.

**Fields**:
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }             // Required, unique
    public string PasswordHash { get; set; }      // Hashed password (Identity framework)
    public string Role { get; set; }              // "Admin", "Waiter", "Chef", "Guest"
    public string DisplayName { get; set; }       // Required, max 100 chars
    public string Phone { get; set; }             // Optional, max 20 chars
    public bool IsActive { get; set; }            // FR-007
    public bool IsDeleted { get; set; }           // Soft delete for GDPR (FR-114)
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public Staff Staff { get; set; }              // Nullable for Guest users
    public LoyaltyAccount LoyaltyAccount { get; set; } // Nullable
}
```

**Validation** (FR-001, FR-002, FR-120):
- Email: Required, valid email format, unique
- Password: Min 10 chars, mixed case, number, special char
- Role: One of ["Admin", "Waiter", "Chef", "Guest"]
- DisplayName: Required, 2-100 characters

**Indexes**:
- Primary: Id
- Unique: Email

**State Transitions**:
- IsActive: true → false (deactivation by admin, FR-007)
- IsDeleted: false → true (anonymization on deletion request, FR-114)

---

### Staff

**Purpose**: Extended information for employee users (Waiter, Chef).

**Fields**:
```csharp
public class Staff
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }              // One-to-one with User
    public string Role { get; set; }              // "Waiter" or "Chef"
    public string WorkStation { get; set; }       // "Bar", "Grill", "Dessert" (for Chefs)
    public DateTime? ShiftStartAt { get; set; }   // FR-101
    public DateTime? ShiftEndAt { get; set; }

    // Navigation properties
    public User User { get; set; }
}
```

**Validation** (FR-100):
- UserId: Must reference existing User with role "Waiter" or "Chef"
- Role: Must match User.Role
- WorkStation: Optional for Waiters, recommended for Chefs

**Indexes**:
- Primary: Id
- Unique: UserId

---

### MenuCategory

**Purpose**: Logical grouping of menu items.

**Fields**:
```csharp
public class MenuCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // Required, max 100 chars
    public int Order { get; set; }                // Display order (FR-020)
    public bool IsDeleted { get; set; }           // Soft delete

    // Navigation properties
    public ICollection<MenuItem> MenuItems { get; set; }
}
```

**Validation** (FR-015):
- Name: Required, 1-100 characters, unique
- Order: Non-negative integer

**Indexes**:
- Primary: Id
- Index: Order

---

### MenuItem

**Purpose**: Food or beverage item available for ordering.

**Fields**:
```csharp
public class MenuItem
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; }              // Required, max 150 chars
    public string Description { get; set; }       // Optional, max 500 chars
    public decimal Price { get; set; }            // Base price
    public string SKU { get; set; }               // Optional, max 50 chars
    public decimal TaxRate { get; set; }          // Percentage (e.g., 0.08 for 8%)
    public bool IsAvailable { get; set; }         // FR-017
    public string ImagePath { get; set; }         // Optional, URL or path
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public MenuCategory Category { get; set; }
    public ICollection<MenuItemOption> Options { get; set; }
    public ICollection<Recipe> Recipes { get; set; } // BOM for inventory (FR-067)
}
```

**Validation** (FR-016):
- Name: Required, 1-150 characters
- Price: Non-negative, max 2 decimal places
- TaxRate: 0.0 - 1.0
- ImagePath: Valid URL or relative path

**Indexes**:
- Primary: Id
- Composite: (CategoryId, IsAvailable)

**State Transitions**:
- IsAvailable: Toggled by admin (FR-017)

---

### MenuItemOption

**Purpose**: Customization option for a menu item (e.g., "Size", "Protein").

**Fields**:
```csharp
public class MenuItemOption
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string Name { get; set; }              // "Size", "Protein", "Extras"
    public string Type { get; set; }              // "Single" or "Multi"
    public bool Required { get; set; }            // FR-018

    // Navigation properties
    public MenuItem MenuItem { get; set; }
    public ICollection<MenuItemModifier> Modifiers { get; set; }
}
```

**Validation** (FR-018):
- Name: Required, 1-100 characters
- Type: "Single" (radio) or "Multi" (checkbox)

---

### MenuItemModifier

**Purpose**: Specific choice within an option (e.g., "Large +$2").

**Fields**:
```csharp
public class MenuItemModifier
{
    public Guid Id { get; set; }
    public Guid OptionId { get; set; }
    public string Name { get; set; }              // "Large", "Extra Cheese"
    public decimal PriceDelta { get; set; }       // Can be negative for discounts

    // Navigation properties
    public MenuItemOption Option { get; set; }
}
```

**Validation** (FR-019):
- Name: Required, 1-100 characters
- PriceDelta: Can be positive, negative, or zero

---

### InventoryItem

**Purpose**: Stock-keeping unit tracked in inventory.

**Fields**:
```csharp
public class InventoryItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // "Chicken Breast", "Tomato"
    public string SKU { get; set; }               // Optional, max 50 chars
    public decimal StockQty { get; set; }         // Can go negative (FR-edge case)
    public decimal ReorderLevel { get; set; }     // Threshold for alerts (FR-064)
    public string Unit { get; set; }              // "kg", "liters", "units"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<InventoryAdjustment> Adjustments { get; set; }
}
```

**Validation** (FR-061):
- Name: Required, 1-150 characters
- ReorderLevel: Non-negative
- Unit: Required (for UX clarity)

**Business Rules**:
- StockQty can go negative (tracks backorders, FR-edge case)
- Alert triggered when StockQty < ReorderLevel (FR-064)

---

### Recipe (Join Table)

**Purpose**: Bill-of-materials linking MenuItem to InventoryItems.

**Fields**:
```csharp
public class Recipe
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public Guid InventoryItemId { get; set; }
    public decimal Quantity { get; set; }         // Amount consumed per order

    // Navigation properties
    public MenuItem MenuItem { get; set; }
    public InventoryItem InventoryItem { get; set; }
}
```

**Validation** (FR-067):
- Quantity: Positive, non-zero

**Example**:
- MenuItem: "Cheeseburger"
- Recipe entries: (Beef Patty, 0.2kg), (Cheese, 0.05kg), (Bun, 1 unit)

---

### Order

**Purpose**: Customer order transaction.

**Fields**:
```csharp
public class Order
{
    public Guid Id { get; set; }
    public Guid? TableId { get; set; }            // Nullable for takeout/delivery
    public Guid? GuestId { get; set; }            // Nullable for walk-in orders
    public string Status { get; set; }            // "Placed", "Confirmed", "InKitchen", "Ready", "Served", "Completed", "Cancelled"
    public string PaymentStatus { get; set; }     // "Unpaid", "PartiallyPaid", "Paid", "Refunded"
    public decimal TotalAmount { get; set; }      // Denormalized (includes modifiers + tax)
    public decimal TaxAmount { get; set; }        // Calculated
    public string Notes { get; set; }             // Optional, waiter notes (FR-029)
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public Table Table { get; set; }
    public User Guest { get; set; }
    public ICollection<OrderLine> OrderLines { get; set; }
    public ICollection<Payment> Payments { get; set; }
}
```

**Validation** (FR-023, FR-027):
- Status: Valid state (see state machine below)
- TotalAmount: Sum of OrderLines (with modifiers) + tax
- TaxAmount: Calculated from menu item tax rates

**Indexes**:
- Primary: Id
- Composite: (Status, CreatedAt) for filtering (FR-032)

**State Machine** (FR-026):
```
Placed → Confirmed → InKitchen → Ready → Served → Completed
   ↓                                              ↓
 Cancelled ←──────────────────────────────────────┘
```

**State Transitions**:
- Placed → Confirmed: Waiter/system confirms order
- Confirmed → InKitchen: Sent to kitchen
- InKitchen → Ready: Chef marks all items ready
- Ready → Served: Waiter delivers to table
- Served → Completed: Payment completed
- Any → Cancelled: Allowed before "Ready" (FR-030)

---

### OrderLine

**Purpose**: Individual item within an order.

**Fields**:
```csharp
public class OrderLine
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }            // Snapshot at order time
    public string ModifiersJson { get; set; }     // JSON: [{ "optionId": "", "modifierId": "", "name": "", "priceDelta": 0 }]
    public string Status { get; set; }            // "Pending", "InProgress", "Ready" (FR-035)

    // Navigation properties
    public Order Order { get; set; }
    public MenuItem MenuItem { get; set; }
}
```

**Validation** (FR-024):
- Quantity: Positive integer
- Price: Matches MenuItem.Price at order time (historical accuracy)
- ModifiersJson: Valid JSON array

**Why JSON for Modifiers?** (FR-024)
- Flexibility: Modifiers can change after order (menu updates)
- Historical accuracy: Snapshot preserves what customer ordered
- Query simplicity: No complex join table needed

---

### Table

**Purpose**: Physical table in the restaurant.

**Fields**:
```csharp
public class Table
{
    public Guid Id { get; set; }
    public string Number { get; set; }            // "1", "A5", etc.
    public int Seats { get; set; }
    public string Status { get; set; }            // "Available", "Occupied", "Reserved", "Cleaning"
    public decimal? PositionX { get; set; }       // For floor plan editor (FR-044)
    public decimal? PositionY { get; set; }

    // Navigation properties
    public ICollection<Order> Orders { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
}
```

**Validation** (FR-040):
- Number: Required, unique
- Seats: Positive integer
- Status: Valid state

**State Machine** (FR-041):
```
Available ⇄ Reserved
    ↓           ↓
  Occupied ⇄ Cleaning → Available
```

---

### Reservation

**Purpose**: Future table booking.

**Fields**:
```csharp
public class Reservation
{
    public Guid Id { get; set; }
    public string GuestName { get; set; }
    public string GuestEmail { get; set; }
    public string GuestPhone { get; set; }
    public int PartySize { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }           // Calculated from duration
    public Guid? TableId { get; set; }            // Assigned table (FR-047)
    public string Status { get; set; }            // "Pending", "Confirmed", "Seated", "Completed", "Cancelled", "NoShow"
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Table Table { get; set; }
}
```

**Validation** (FR-045):
- GuestName: Required, 2-100 characters
- GuestEmail or GuestPhone: At least one required
- PartySize: Positive integer
- StartAt: Future date/time
- EndAt: After StartAt

**Business Rules** (FR-050):
- Prevent double-booking: Check for overlapping reservations on same table
- Allow configurable overbooking percentage (e.g., +10%)

**State Machine** (FR-048):
```
Pending → Confirmed → Seated → Completed
   ↓          ↓
Cancelled ← NoShow
```

---

### Payment

**Purpose**: Payment transaction for an order.

**Fields**:
```csharp
public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Method { get; set; }            // "Cash", "Card", "Terminal"
    public decimal Amount { get; set; }
    public string TransactionId { get; set; }     // External provider ID (Stripe, Square)
    public string Status { get; set; }            // "Pending", "Completed", "Failed", "Refunded"
    public decimal? TipAmount { get; set; }       // Optional (FR-055)
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public Order Order { get; set; }
}
```

**Validation** (FR-053):
- Method: "Cash", "Card", or "Terminal"
- Amount: Positive, <= Order.TotalAmount
- TransactionId: Required for "Card" method

**Business Rules** (FR-054):
- Multiple payments allowed (split checks)
- Sum of payments can exceed TotalAmount (tips)

---

### Printer

**Purpose**: Configured printer device.

**Fields**:
```csharp
public class Printer
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // "Kitchen Grill", "Receipt"
    public string Type { get; set; }              // "Receipt" or "Kitchen"
    public string ConnectionString { get; set; }  // IP:port for network printers
    public bool IsOnline { get; set; }            // Updated by health check
    public DateTime? LastPrintAt { get; set; }
}
```

**Validation** (FR-074):
- Name: Required, unique
- Type: "Receipt" or "Kitchen"
- ConnectionString: Valid format (IP:port)

---

### KitchenStation

**Purpose**: Kitchen preparation area for routing orders.

**Fields**:
```csharp
public class KitchenStation
{
    public Guid Id { get; set; }
    public string Name { get; set; }              // "Grill", "Fryer", "Dessert"
    public Guid? PrinterId { get; set; }          // Optional dedicated printer

    // Navigation properties
    public Printer Printer { get; set; }
}
```

**Validation** (FR-037):
- Name: Required, unique

**Routing Logic** (FR-037):
- MenuItem can have KitchenStationId (configurable per item)
- OrderLines routed to appropriate station based on MenuItem

---

### LoyaltyAccount

**Purpose**: Guest loyalty program participation.

**Fields**:
```csharp
public class LoyaltyAccount
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }              // One-to-one with User (Guest role)
    public int Points { get; set; }               // Earned points (FR-068)
    public string Tier { get; set; }              // "Bronze", "Silver", "Gold"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; }
}
```

**Validation** (FR-069):
- UserId: Must reference User with role "Guest"
- Points: Non-negative integer
- Tier: Valid tier name

**Business Rules** (FR-071):
- Points earned: Configurable rate (e.g., 1 point per $1 spent)
- Points redeemed: Conversion rate defined in settings (e.g., 100 points = $5 discount)

---

### AuditLog

**Purpose**: Immutable record of all significant changes.

**Fields**:
```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public string EntityType { get; set; }        // "Order", "MenuItem", etc.
    public string EntityId { get; set; }          // Guid as string
    public string Action { get; set; }            // "Created", "Updated", "Deleted"
    public Guid? UserId { get; set; }             // Who performed the action
    public string ChangesSummary { get; set; }    // JSON: { "field": { "old": "", "new": "" } }
    public DateTime Timestamp { get; set; }

    // Navigation properties
    public User User { get; set; }
}
```

**Validation** (FR-095):
- EntityType: Required
- Action: "Created", "Updated", or "Deleted"
- Timestamp: Automatically set on insert

**Indexes**:
- Primary: Id
- Composite: (EntityType, Timestamp) for filtering (FR-096)

**Immutability** (FR-098):
- No UPDATE or DELETE allowed on this table
- Append-only (enforced by DB constraints or app logic)

---

## Supporting Entities (Not in spec, but needed)

### InventoryAdjustment

**Purpose**: Track manual inventory adjustments.

```csharp
public class InventoryAdjustment
{
    public Guid Id { get; set; }
    public Guid InventoryItemId { get; set; }
    public decimal QuantityChange { get; set; }   // Can be +/-
    public string Reason { get; set; }            // "Received shipment", "Spoilage"
    public Guid UserId { get; set; }              // Admin who adjusted
    public DateTime CreatedAt { get; set; }

    public InventoryItem InventoryItem { get; set; }
    public User User { get; set; }
}
```

### MenuItemTranslation

**Purpose**: Localized menu item names/descriptions (FR-112).

```csharp
public class MenuItemTranslation
{
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public string LanguageCode { get; set; }      // "en", "es", "fr"
    public string Name { get; set; }
    public string Description { get; set; }

    public MenuItem MenuItem { get; set; }
}
```

---

## Validation Summary

All entities include:
- **Soft Delete**: `IsDeleted` flag for recovery and audit (where applicable)
- **Timestamps**: `CreatedAt`, `UpdatedAt` for tracking

## Next Steps

1. Generate EF Core entity configurations (fluent API)
2. Create initial migration
3. Define API contracts (DTOs, endpoints) in `/contracts/`
4. Generate contract tests (failing tests for TDD)
