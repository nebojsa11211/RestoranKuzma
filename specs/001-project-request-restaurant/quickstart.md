# Quickstart Guide

**Feature**: Restaurant Management & Ordering Suite
**Branch**: 001-project-request-restaurant
**Date**: 2025-10-06

## Purpose
This quickstart demonstrates the core order lifecycle: Guest places order → Kitchen processes → Waiter receives notification → Payment → Completion. This validates the primary user stories and acceptance criteria from the specification.

---

## Prerequisites

- .NET 9 SDK installed
- Docker Desktop running (for PostgreSQL + Redis)
- Visual Studio 2022 or VS Code with C# extension
- git repository cloned

---

## 1. Initial Setup (5 minutes)

### Clone and Navigate
```bash
git clone <repository-url>
cd RestoranKuzma
git checkout 001-project-request-restaurant
```

### Start Infrastructure
```bash
cd docker
docker-compose up -d
```

This starts:
- PostgreSQL on `localhost:5432`
- Redis on `localhost:6379`

### Run Database Migrations
```bash
cd ../src/RestaurantSuite.Api
dotnet ef database update
```

This creates the database schema and seeds initial data for one restaurant: "Restoran Kuzma"
- 1 Restaurant: "Restoran Kuzma"
- 4 Users:
  - `admin@demo.com` / `Admin123!@#` (Admin)
  - `waiter@demo.com` / `Waiter123!@#` (Waiter)
  - `chef@demo.com` / `Chef123!@#` (Chef)
  - `guest@demo.com` / `Guest123!@#` (Guest)
- 5 Menu Items:
  - Cheeseburger ($12.99)
  - Caesar Salad ($8.99)
  - Grilled Chicken ($15.99)
  - Chocolate Cake ($6.99)
  - Soda ($2.99)
- 3 Tables: Table 1 (4 seats), Table 2 (2 seats), Table 3 (6 seats)

### Build Solution
```bash
dotnet build ../../RestaurantSuite.sln
```

---

## 2. Start Applications (2 minutes)

Open 4 terminal windows:

**Terminal 1: API**
```bash
cd src/RestaurantSuite.Api
dotnet run
```
API available at `https://localhost:5000`

**Terminal 2: Admin Dashboard**
```bash
cd src/RestaurantSuite.Admin
dotnet run
```
Admin UI at `https://localhost:5001`

**Terminal 3: Waiter App**
```bash
cd src/RestaurantSuite.Waiter
dotnet run
```
Waiter PWA at `https://localhost:5002`

**Terminal 4: Chef App**
```bash
cd src/RestaurantSuite.Chef
dotnet run
```
Chef PWA at `https://localhost:5003`

---

## 3. Core Workflow Validation (10 minutes)

### Step 1: Login as Guest
1. Open browser: `https://localhost:5004` (Guest app)
2. Login:
   - Email: `guest@demo.com`
   - Password: `Guest123!@#`
3. ✅ **Verify**: User is authenticated and sees menu

### Step 2: Browse Menu (FR-022)
1. Navigate to Menu page
2. ✅ **Verify**: See 5 menu items with images and prices
3. ✅ **Verify**: Items grouped by category (Mains, Sides, Desserts, Drinks)

### Step 3: Place Order (FR-023)
1. Select "Cheeseburger" → Add to cart
2. Click modifiers: Select "No onions" (no price change)
3. Select "Caesar Salad" → Add to cart
4. Select Table 1 from dropdown
5. Add note: "Extra napkins please"
6. Click "Place Order"
7. ✅ **Verify**: Order created successfully
8. ✅ **Verify**: Order status = "Placed"
9. ✅ **Verify**: Order total = $21.98 + tax

### Step 4: Kitchen Receives Order (FR-082)
1. Switch to Terminal 4 (Chef app browser at `https://localhost:5003`)
2. Login as `chef@demo.com` / `Chef123!@#`
3. ✅ **Verify**: New order appears in Kitchen Display within 2 seconds (FR-085)
4. ✅ **Verify**: Order shows:
   - Table 1
   - Cheeseburger (No onions)
   - Caesar Salad
   - Note: "Extra napkins please"
5. ✅ **Verify**: Order status = "InKitchen"

### Step 5: Chef Prepares Order (FR-035)
1. Click "Cheeseburger" item → Mark as "In Progress"
2. ✅ **Verify**: Item status changes to "In Progress"
3. Wait 5 seconds (simulate cooking)
4. Click "Cheeseburger" → Mark as "Ready"
5. ✅ **Verify**: Item status changes to "Ready"
6. Repeat for "Caesar Salad"
7. ✅ **Verify**: When all items ready, order status changes to "Ready"

### Step 6: Waiter Notified (FR-083)
1. Switch to Terminal 3 (Waiter app browser at `https://localhost:5002`)
2. Login as `waiter@demo.com` / `Waiter123!@#`
3. ✅ **Verify**: Notification appears: "Order for Table 1 is ready"
4. ✅ **Verify**: Notification received within 2 seconds (FR-085)
5. Navigate to Orders page
6. Find order for Table 1
7. ✅ **Verify**: Order status = "Ready"

### Step 7: Waiter Serves Order (FR-028)
1. Click on Table 1 order
2. Click "Mark as Served"
3. ✅ **Verify**: Order status changes to "Served"
4. ✅ **Verify**: "Process Payment" button appears

### Step 8: Process Payment (FR-053)
1. Click "Process Payment"
2. Select payment method: "Cash"
3. Enter amount: $25.00
4. Enter tip: $3.00
5. Click "Complete Payment"
6. ✅ **Verify**: Payment recorded successfully
7. ✅ **Verify**: Receipt printed (check logs for print job)
8. ✅ **Verify**: Order status changes to "Completed"
9. ✅ **Verify**: Order paymentStatus = "Paid"

### Step 9: Inventory Updated (FR-063)
1. Switch to Terminal 2 (Admin browser at `https://localhost:5001`)
2. Login as `admin@demo.com` / `Admin123!@#`
3. Navigate to Inventory page
4. ✅ **Verify**: Ingredients for Cheeseburger and Caesar Salad are deducted
   - Beef Patty: -0.2 kg
   - Cheese: -0.05 kg
   - Lettuce: -0.1 kg

### Step 10: View Reports (FR-088)
1. In Admin UI, navigate to Reports → Sales
2. Select date range: Today
3. ✅ **Verify**: Report shows:
   - Total sales: $21.98
   - Order count: 1
   - Average order value: $21.98
4. ✅ **Verify**: Export to CSV works

---

## 4. Real-Time Validation (5 minutes)

### Multi-Device Synchronization
1. Open 2 browser windows side-by-side:
   - Window 1: Chef app (logged in)
   - Window 2: Waiter app (logged in)
2. Create new order as Guest (repeat Steps 1-3)
3. ✅ **Verify**: Order appears in Chef window within 2 seconds
4. In Chef window, mark items as "Ready"
5. ✅ **Verify**: Waiter window receives notification within 2 seconds
6. ✅ **Verify**: Real-time sync works across multiple clients (FR-081)

### SignalR Reconnection (FR-086)
1. Stop API (Ctrl+C in Terminal 1)
2. ✅ **Verify**: Waiter/Chef apps show "Disconnected" status
3. Restart API
4. ✅ **Verify**: Apps automatically reconnect within 5 seconds
5. ✅ **Verify**: Missed updates are not lost (check order status consistency)

---

## 5. Offline PWA Test (5 minutes)

### Install PWA (FR-104)
1. In Waiter app (`https://localhost:5002`), click browser's "Install" button
2. ✅ **Verify**: App installs as standalone PWA

### Offline Menu Browse (FR-106)
1. In installed Waiter PWA, navigate to Menu
2. Open browser DevTools → Network → Check "Offline"
3. ✅ **Verify**: Menu still displays (cached)
4. Try to place order
5. ✅ **Verify**: Message: "You are offline. Order will sync when online."
6. Uncheck "Offline"
7. ✅ **Verify**: Order syncs automatically (FR-107)

---

## 6. Edge Cases (5 minutes)

### Unavailable Menu Item (Edge Case 1)
1. In Admin UI, edit "Cheeseburger"
2. Set "IsAvailable" = false
3. In Guest app, refresh menu
4. ✅ **Verify**: "Cheeseburger" shows as "Unavailable"
5. Try to add to cart
6. ✅ **Verify**: Error: "This item is currently unavailable"

### Order Cancellation (FR-030)
1. Place new order as Guest
2. In Waiter app, find the order
3. Click "Cancel Order"
4. Enter reason: "Customer left"
5. ✅ **Verify**: Order status = "Cancelled"
6. ✅ **Verify**: Kitchen receives cancellation notification
7. ✅ **Verify**: Inventory reversal (items added back to stock)

### Payment Failure Handling (Edge Case 2)
1. Place order and mark as "Served"
2. In Waiter app, attempt payment with invalid card (use test card: `4000000000000341`)
3. ✅ **Verify**: Payment fails gracefully
4. ✅ **Verify**: Error message: "Payment failed. Please try again or use alternative method."
5. Retry with "Cash" method
6. ✅ **Verify**: Payment succeeds

### Low Stock Alert (FR-064)
1. In Admin UI, navigate to Inventory
2. Set "Beef Patty" stock to 0.5 kg (below reorder level of 5 kg)
3. ✅ **Verify**: Alert badge appears on Inventory menu
4. ✅ **Verify**: Real-time notification: "Low stock: Beef Patty"

---

## 7. Performance Validation (5 minutes)

### Load Test (Basic)
Using PowerShell:
```powershell
# Install k6 (load testing tool)
choco install k6

# Run load test script
k6 run tests/load-test.js
```

Load test script (`tests/load-test.js`):
```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '1m', target: 50 },  // Ramp up to 50 users
    { duration: '2m', target: 50 },  // Stay at 50 users
    { duration: '1m', target: 0 },   // Ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% of requests < 500ms (FR-117)
  },
};

export default function () {
  // GET /api/menu
  let menuRes = http.get('https://localhost:5000/api/menu');
  check(menuRes, { 'menu loaded': (r) => r.status === 200 });

  sleep(1);

  // POST /api/orders
  let orderPayload = JSON.stringify({
    tableId: 'table-1-guid',
    orderLines: [
      { menuItemId: 'burger-guid', quantity: 1, modifiers: [] }
    ]
  });
  let orderRes = http.post('https://localhost:5000/api/orders', orderPayload, {
    headers: { 'Content-Type': 'application/json' },
  });
  check(orderRes, { 'order created': (r) => r.status === 201 });

  sleep(1);
}
```

✅ **Verify**: p95 latency < 500ms for critical operations

---

## 8. Health & Monitoring (2 minutes)

### Health Check (FR-124)
```bash
curl https://localhost:5000/health
```

✅ **Verify**: Response:
```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "redis": "Healthy",
    "signalr": "Healthy"
  }
}
```

### Metrics (FR-126)
```bash
curl https://localhost:5000/metrics
```

✅ **Verify**: Prometheus-format metrics returned:
- `http_requests_total`
- `order_placement_duration_seconds`
- `signalr_connection_count`

---

## 9. Cleanup

### Stop Applications
Press Ctrl+C in all terminal windows

### Stop Docker Containers
```bash
cd docker
docker-compose down
```

### (Optional) Reset Database
```bash
cd ../src/RestaurantSuite.Api
dotnet ef database drop --force
dotnet ef database update
```

---

## Summary of Validated Requirements

### Functional Requirements
- ✅ FR-001: User registration and login
- ✅ FR-022: Guest menu browsing
- ✅ FR-023: Order creation
- ✅ FR-026: Order status lifecycle
- ✅ FR-028: Waiter/Chef status updates
- ✅ FR-030: Order cancellation
- ✅ FR-035: Chef marks items in progress/ready
- ✅ FR-053: Payment processing
- ✅ FR-063: Inventory deduction
- ✅ FR-064: Low stock alerts
- ✅ FR-081: Real-time order status broadcast
- ✅ FR-082: Kitchen notifications
- ✅ FR-083: Waiter notifications
- ✅ FR-085: <2s notification delivery
- ✅ FR-086: Automatic reconnection
- ✅ FR-104: PWA installable
- ✅ FR-106: Offline menu viewing
- ✅ FR-107: Offline sync

### Acceptance Scenarios
- ✅ Order Lifecycle (Scenarios 1-4)
- ✅ Menu Management (Scenarios 5-6)
- ✅ Inventory Management (Scenarios 9-10)
- ✅ Real-time Notifications (Scenarios 15-16)
- ✅ Offline & PWA Capabilities (Scenarios 17-18)

### Edge Cases
- ✅ Unavailable menu item
- ✅ Payment failure handling
- ✅ Order cancellation with inventory reversal
- ✅ Low stock alert

---

## Troubleshooting

### "Database connection failed"
- Ensure Docker is running: `docker ps`
- Check PostgreSQL container: `docker logs restaurantsuite_postgres`
- Verify connection string in `appsettings.Development.json`

### "SignalR connection failed"
- Ensure Redis is running: `docker ps`
- Check Redis logs: `docker logs restaurantsuite_redis`
- Verify CORS policy allows frontend origin

### "Menu items not appearing"
- Run seed script: `dotnet run --seed` in API project
- Check logs for migration errors
- Verify user is logged in with correct credentials

### "Real-time updates not working"
- Check browser console for SignalR connection errors
- Verify JWT token is valid (check `/api/auth/refresh`)
- Ensure user has joined correct SignalR groups

---

## Next Steps

After completing this quickstart:
1. Run full test suite: `dotnet test`
2. Review generated API documentation: `https://localhost:5000/swagger`
3. Explore admin dashboard features (staff management, reports, settings)
4. Test reservation flow
5. Test loyalty program
6. Deploy to staging environment using Docker Compose

---

## Support

For issues or questions:
- Check logs: `src/RestaurantSuite.Api/logs/`
- Review API documentation: `/swagger`
- Consult spec: `specs/001-project-request-restaurant/spec.md`
- Contact development team
