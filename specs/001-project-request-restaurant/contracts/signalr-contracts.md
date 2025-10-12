# SignalR Hub Contracts

**Feature**: Restaurant Management & Ordering Suite
**Branch**: 001-project-request-restaurant
**Date**: 2025-10-06

## Overview
This document defines the SignalR hub interfaces for real-time communication in the restaurant system. All hubs use strongly-typed client interfaces for compile-time safety.

---

## Hub URLs

- **Orders Hub**: `/hubs/orders` - Broadcast order status changes (FR-081)
- **Kitchen Hub**: `/hubs/kitchen` - Kitchen-specific updates (FR-082)
- **Notifications Hub**: `/hubs/notifications` - General notifications (FR-083)

---

## Orders Hub

**Purpose**: Real-time order lifecycle events for waiters and kitchen staff.

**Hub Path**: `/hubs/orders`

**Authorization**: Requires `Waiter`, `Chef`, or `Admin` role

### Server → Client Methods

```csharp
public interface IOrderClient
{
    /// <summary>
    /// Notifies clients when a new order is created (FR-081)
    /// </summary>
    /// <param name="order">Created order details</param>
    Task OrderCreated(OrderDto order);

    /// <summary>
    /// Notifies clients when order status changes (FR-081)
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="newStatus">New status value</param>
    /// <param name="updatedBy">User who updated</param>
    Task OrderStatusChanged(Guid orderId, OrderStatus newStatus, string updatedBy);

    /// <summary>
    /// Notifies clients when an order line status changes (FR-035)
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="orderLineId">Specific order line ID</param>
    /// <param name="newStatus">New status (Pending/InProgress/Ready)</param>
    Task OrderLineStatusChanged(Guid orderId, Guid orderLineId, string newStatus);

    /// <summary>
    /// Notifies clients when an order is cancelled (FR-030)
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="reason">Cancellation reason</param>
    Task OrderCancelled(Guid orderId, string reason);

    /// <summary>
    /// Notifies clients when payment is completed (FR-053)
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="paymentDetails">Payment summary</param>
    Task PaymentCompleted(Guid orderId, PaymentDto paymentDetails);
}
```

### Client → Server Methods

```csharp
public class OrdersHub : Hub<IOrderClient>
{
    /// <summary>
    /// Client subscribes to updates for a specific table
    /// </summary>
    public async Task JoinTable(Guid tableId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"table:{tableId}");
    }

    /// <summary>
    /// Client unsubscribes from table updates
    /// </summary>
    public async Task LeaveTable(Guid tableId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"table:{tableId}");
    }

    /// <summary>
    /// Client subscribes to all orders for the kitchen view
    /// </summary>
    public async Task JoinKitchen()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "kitchen");
    }
}
```

### Message Examples

#### OrderCreated Event
```json
{
  "order": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tableNumber": "5",
    "status": "Placed",
    "totalAmount": 45.50,
    "orderLines": [
      {
        "id": "1234-5678",
        "menuItemName": "Cheeseburger",
        "quantity": 2,
        "modifiers": [
          { "name": "No onions", "priceDelta": 0 }
        ]
      }
    ],
    "createdAt": "2025-10-06T14:30:00Z"
  }
}
```

#### OrderStatusChanged Event
```json
{
  "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "newStatus": "Ready",
  "updatedBy": "Chef Mario"
}
```

---

## Kitchen Hub

**Purpose**: Kitchen-specific real-time updates.

**Hub Path**: `/hubs/kitchen`

**Authorization**: Requires `Chef` or `Admin` role

### Server → Client Methods

```csharp
public interface IKitchenClient
{
    /// <summary>
    /// Notifies kitchen when a new order arrives (FR-082)
    /// </summary>
    /// <param name="ticket">Kitchen ticket with order details</param>
    Task NewTicket(KitchenTicketDto ticket);

    /// <summary>
    /// Notifies kitchen when an order is modified (FR-033)
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="changes">List of changes (added/removed items)</param>
    Task OrderModified(Guid orderId, List<string> changes);

    /// <summary>
    /// Notifies kitchen when an order is cancelled (FR-030)
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="reason">Cancellation reason</param>
    Task OrderCancelled(Guid orderId, string reason);

    /// <summary>
    /// Alerts kitchen when an order is delayed (FR-edge case)
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="minutesWaiting">Minutes since order placed</param>
    Task OrderDelayAlert(Guid orderId, int minutesWaiting);
}
```

### Client → Server Methods

```csharp
public class KitchenHub : Hub<IKitchenClient>
{
    /// <summary>
    /// Client subscribes to tickets for a specific kitchen station
    /// </summary>
    public async Task JoinStation(Guid kitchenStationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"station:{kitchenStationId}");
    }

    /// <summary>
    /// Mark order line as in progress
    /// </summary>
    public async Task MarkInProgress(Guid orderLineId)
    {
        // Business logic to update status
        // Then broadcast to other connected kitchens
    }

    /// <summary>
    /// Mark order line as ready
    /// </summary>
    public async Task MarkReady(Guid orderLineId)
    {
        // Business logic to update status
        // Trigger notification to waiters
    }
}
```

### Message Examples

#### NewTicket Event
```json
{
  "ticket": {
    "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tableNumber": "5",
    "orderNumber": 42,
    "items": [
      {
        "name": "Cheeseburger",
        "quantity": 2,
        "modifiers": ["No onions"],
        "status": "Pending"
      }
    ],
    "station": "Grill",
    "createdAt": "2025-10-06T14:30:00Z"
  }
}
```

---

## Notifications Hub

**Purpose**: General notifications for all users.

**Hub Path**: `/hubs/notifications`

**Authorization**: Authenticated users

### Server → Client Methods

```csharp
public interface INotificationClient
{
    /// <summary>
    /// Generic notification message (FR-083)
    /// </summary>
    /// <param name="notification">Notification details</param>
    Task ReceiveNotification(NotificationDto notification);

    /// <summary>
    /// Low stock alert for admins (FR-064)
    /// </summary>
    /// <param name="itemName">Inventory item name</param>
    /// <param name="currentQty">Current quantity</param>
    /// <param name="reorderLevel">Reorder threshold</param>
    Task LowStockAlert(string itemName, decimal currentQty, decimal reorderLevel);

    /// <summary>
    /// Printer offline alert (FR-079)
    /// </summary>
    /// <param name="printerName">Name of offline printer</param>
    Task PrinterOfflineAlert(string printerName);
}
```

### Client → Server Methods

```csharp
public class NotificationsHub : Hub<INotificationClient>
{
    /// <summary>
    /// Client subscribes to general notifications
    /// </summary>
    public async Task JoinNotifications()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "all");
    }

    /// <summary>
    /// Client subscribes to role-specific notifications
    /// </summary>
    public async Task JoinRoleGroup(string role)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"role:{role}");
    }
}
```

### Message Examples

#### ReceiveNotification Event
```json
{
  "notification": {
    "id": "abc-123",
    "type": "OrderReady",
    "title": "Order #42 Ready",
    "message": "Table 5's order is ready for service",
    "severity": "Info",
    "timestamp": "2025-10-06T14:35:00Z",
    "actionUrl": "/orders/3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
}
```

#### LowStockAlert Event
```json
{
  "itemName": "Chicken Breast",
  "currentQty": 2.5,
  "reorderLevel": 5.0
}
```

---

## Connection Lifecycle

### Client Connection Flow

1. **Authenticate**: Client sends JWT token in connection query string or header
   ```javascript
   const connection = new signalR.HubConnectionBuilder()
     .withUrl("/hubs/orders", { accessTokenFactory: () => jwtToken })
     .build();
   ```

2. **Join Groups**: Client calls appropriate join methods
   ```javascript
   await connection.invoke("JoinKitchen");
   await connection.invoke("JoinNotifications");
   ```

3. **Register Handlers**: Client registers callback functions
   ```javascript
   connection.on("OrderCreated", (order) => {
     console.log("New order:", order);
   });
   ```

4. **Handle Reconnection**: Automatic reconnection with exponential backoff (FR-086)
   ```javascript
   connection.onreconnected(() => {
     // Re-join groups after reconnection
     connection.invoke("JoinKitchen");
     connection.invoke("JoinNotifications");
   });
   ```

### Server Broadcasting Patterns

#### Broadcast to All Clients
```csharp
await Clients.Group("all")
    .ReceiveNotification(notification);
```

#### Broadcast to Specific Table
```csharp
await Clients.Group($"table:{tableId}")
    .OrderStatusChanged(orderId, newStatus, updatedBy);
```

#### Broadcast to Kitchen
```csharp
await Clients.Group("kitchen")
    .NewTicket(ticket);
```

#### Broadcast to Kitchen Station
```csharp
await Clients.Group($"station:{kitchenStationId}")
    .NewTicket(ticket);
```

#### Broadcast to Specific Role
```csharp
await Clients.Group($"role:Waiter")
    .OrderReady(orderId);
```

---

## Performance Targets

- **Message Delivery**: <2 seconds from server event to client notification (FR-085)
- **Concurrent Connections**: Support 500+ WebSocket connections per API instance
- **Scalability**: Redis backplane for cross-instance messaging
- **Reconnection**: Automatic with exponential backoff (1s, 2s, 4s, 8s, 16s, max 30s)

---

## Error Handling

### Client-Side Errors
```javascript
connection.on("error", (error) => {
  console.error("SignalR error:", error);
  // Show user-friendly error message
  // Attempt manual reconnection if automatic fails
});
```

### Server-Side Errors
- Invalid group names: Log and return error to client
- Authorization failures: Disconnect with 401 status
- Hub method exceptions: Log, return error, do not crash hub

---

## Testing

### Contract Tests (TDD)

```csharp
[Fact]
public async Task OrderCreated_BroadcastsToAllKitchenClients()
{
    // Arrange: Mock SignalR hub context
    var mockClients = new Mock<IHubClients<IOrderClient>>();
    var mockGroupManager = new Mock<IGroupManager>();

    // Act: Create order via API
    var response = await _client.PostAsync("/api/orders", orderRequest);

    // Assert: Verify SignalR message sent
    mockClients.Verify(c => c.Group("kitchen")
        .OrderCreated(It.IsAny<OrderDto>()), Times.Once);
}
```

---

## Next Steps

1. Implement typed hub classes in `RestaurantSuite.Notifications` project
2. Configure Redis backplane in `Program.cs`
3. Write integration tests for hub broadcasting
4. Implement client-side SignalR connection in Blazor apps
5. Add metrics for message delivery latency
