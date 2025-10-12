# Restaurant Suite - Admin Frontend Setup

## Overview
The Admin frontend is now fully functional for managing a single restaurant.

## What's Included

### Pages Created:
1. **Dashboard (/)** - Main landing page
   - Overview of restaurant status
   - Quick access to key management areas
   - Links to orders, menu, and table management

2. **Orders (/orders)** - Order management
   - View all orders for the restaurant
   - Status cards showing counts (Pending, In Progress, Ready, Completed)
   - Order table with status badges
   - Update order status workflow
   - Refresh button to reload orders

3. **Menu (/menu)** - Menu management (placeholder)
   - Coming soon page for menu item management

4. **Tables (/tables)** - Table management (placeholder)
   - Coming soon page for table management

### Services:
- **ApiService** - HTTP client service for communicating with the API
  - GetRestaurantSettingsAsync()
  - UpdateRestaurantSettingsAsync(UpdateRestaurantCommand)
  - GetOrdersAsync()
  - CreateOrderAsync(CreateOrderCommand)
  - UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusCommand)

### Configuration:
- HttpClient configured in Program.cs with base URL
- API base URL: `https://localhost:7001` (configurable in appsettings.json)
- Dependency injection setup for ApiService

## How to Run

### Prerequisites:
1. API must be running on `https://localhost:7001`
2. .NET 9.0 SDK installed

### Steps:

1. **Start the API:**
```bash
cd src/RestaurantSuite.Api
dotnet run
```

2. **Start the Admin Frontend (in a new terminal):**
```bash
cd src/RestaurantSuite.Admin
dotnet run
```

3. **Access the Admin Portal:**
   - Open browser to: `https://localhost:5001` or `http://localhost:5000`
   - The default port might vary - check console output

## Configuration

### Change API Base URL:
Edit `src/RestaurantSuite.Admin/appsettings.json`:
```json
{
  "ApiBaseUrl": "https://your-api-url:port"
}
```

## Features

### Restaurant Settings:
- ✅ View and edit restaurant settings
- ✅ Update restaurant configuration
- ✅ Manage restaurant status (Active/Inactive)

### Order Management:
- ✅ View all orders for the restaurant
- ✅ Real-time order counts by status
- ✅ Update order status through dropdown
- ✅ Status workflow (Pending → Confirmed → InProgress → Ready → Served → Completed)
- ✅ Cancel orders
- ✅ Refresh orders manually

### Navigation:
- ✅ Responsive sidebar navigation
- ✅ Dashboard, Menu, Tables, and Settings links
- ✅ Intuitive navigation between pages

## Status Workflow

The order status workflow supported:
```
Pending → Confirm → Confirmed
       → Cancel → Cancelled

Confirmed → Start → InProgress

InProgress → Complete → Ready

Ready → Serve → Served

Served → Finish → Completed
```

## UI Components

All pages use:
- Bootstrap 5 styling
- Responsive grid layout
- Card components
- Form validation
- Loading states
- Error handling

## Next Steps

To fully complete the admin frontend:

1. **Menu Management:**
   - Create menu items
   - Organize categories
   - Set prices and availability
   - Upload images

2. **Table Management:**
   - Create and configure tables
   - View table status
   - Manage reservations

3. **User Management:**
   - Create staff accounts (Waiters, Chefs)
   - Assign roles
   - Manage permissions

4. **Real-time Updates:**
   - Integrate SignalR for live order updates
   - Auto-refresh when orders change
   - Notifications for new orders

5. **Authentication:**
   - Add login page
   - JWT token management
   - Role-based access control
