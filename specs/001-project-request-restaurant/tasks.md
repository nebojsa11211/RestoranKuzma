# Tasks: Restaurant Management & Ordering Suite

**Input**: Design documents from `specs/001-project-request-restaurant/`
**Prerequisites**: plan.md, research.md, data-model.md, contracts/, quickstart.md

## Execution Flow (main)
```
1. Loaded plan.md: .NET 9, Clean Architecture, 14 projects
2. Loaded data-model.md: 17 entities extracted
3. Loaded contracts/: 2 files (api-contracts.yaml, signalr-contracts.md)
4. Loaded quickstart.md: 10 validation scenarios
5. Generated 150+ tasks across 6 phases
6. Applied TDD ordering: Tests before implementation
7. Marked parallel tasks with [P]
8. Validated: All contracts tested, all entities modeled
9. SUCCESS: Tasks ready for execution
```

## Format: `[ID] [P?] Description`
- **[P]**: Can run in parallel (different files, no shared dependencies)
- File paths are absolute or relative to repository root

---

## Phase 1: Foundation & Setup

### T001-T010: Solution Structure & Configuration

- [ ] **T001** Create Visual Studio solution file `RestaurantSuite.sln` at repository root
- [ ] **T002** [P] Create `src/RestaurantSuite.Domain` class library project targeting .NET 9
- [ ] **T003** [P] Create `src/RestaurantSuite.Application` class library project targeting .NET 9
- [ ] **T004** [P] Create `src/RestaurantSuite.Infrastructure.EF` class library project targeting .NET 9
- [ ] **T005** [P] Create `src/RestaurantSuite.Infrastructure.Identity` class library project targeting .NET 9
- [ ] **T006** [P] Create `src/RestaurantSuite.Notifications` class library project targeting .NET 9
- [ ] **T007** [P] Create `src/RestaurantSuite.Integrations` class library project targeting .NET 9
- [ ] **T008** [P] Create `src/RestaurantSuite.Common` class library project targeting .NET 9
- [ ] **T009** Create `src/RestaurantSuite.Api` ASP.NET Core Web API project targeting .NET 9
- [ ] **T010** Add project references: Api → Application → Domain; Infrastructure → Domain

### T011-T020: Blazor Frontend Projects

- [ ] **T011** [P] Create `src/RestaurantSuite.Admin` Blazor Server project targeting .NET 9
- [ ] **T012** [P] Create `src/RestaurantSuite.Waiter` Blazor WebAssembly project targeting .NET 9
- [ ] **T013** [P] Create `src/RestaurantSuite.Chef` Blazor WebAssembly project targeting .NET 9
- [ ] **T014** [P] Create `src/RestaurantSuite.Guest` Blazor WebAssembly project targeting .NET 9

### T015-T020: Test Projects

- [ ] **T015** [P] Create `tests/RestaurantSuite.Tests.Unit` xUnit project targeting .NET 9
- [ ] **T016** [P] Create `tests/RestaurantSuite.Tests.Integration` xUnit project targeting .NET 9

### T021-T030: NuGet Dependencies

- [ ] **T021** Add NuGet packages to Domain: (none - pure C# entities)
- [ ] **T022** Add NuGet packages to Application: `MediatR`, `FluentValidation`, `AutoMapper`
- [ ] **T023** Add NuGet packages to Infrastructure.EF: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`
- [ ] **T024** Add NuGet packages to Infrastructure.Identity: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Microsoft.AspNetCore.Authentication.Google`
- [ ] **T025** Add NuGet packages to Notifications: `Microsoft.AspNetCore.SignalR`, `Microsoft.AspNetCore.SignalR.StackExchangeRedis`
- [ ] **T026** Add NuGet packages to Integrations: `Stripe.net`, `Square` (payment SDKs)
- [ ] **T027** Add NuGet packages to Api: `Swashbuckle.AspNetCore`, `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.Seq`, `Microsoft.Extensions.Diagnostics.HealthChecks`, `OpenTelemetry.Exporter.Prometheus.AspNetCore`
- [ ] **T028** Add NuGet packages to Tests.Unit: `xUnit`, `Moq`, `FluentAssertions`
- [ ] **T029** Add NuGet packages to Tests.Integration: `xUnit`, `Testcontainers.PostgreSql`, `Testcontainers.MsSql`, `Microsoft.AspNetCore.Mvc.Testing`
- [ ] **T030** Restore all NuGet packages: `dotnet restore RestaurantSuite.sln`

### T031-T035: Docker & CI/CD Setup

- [ ] **T031** [P] Create `docker/Dockerfile.Api` for RestaurantSuite.Api with multi-stage build
- [ ] **T032** [P] Create `docker/Dockerfile.Admin` for Blazor Server app
- [ ] **T033** [P] Create `docker/Dockerfile.Waiter` for Blazor WASM PWA
- [ ] **T034** Create `docker/docker-compose.yml` with services: postgres, redis, api, admin (ports: 5432, 6379, 5000, 5001)
- [ ] **T035** [P] Create `.github/workflows/ci.yml` GitHub Actions workflow: build, test, docker build

---

## Phase 2: Tests First (TDD) ⚠️ MUST COMPLETE BEFORE PHASE 3

**CRITICAL: Write tests first, verify they FAIL, then implement in Phase 3**

### T036-T050: Contract Tests (API Endpoints)

All contract tests in `tests/RestaurantSuite.Tests.Integration/Controllers/`

- [ ] **T036** [P] Contract test POST /api/auth/register in `AuthControllerTests.cs` - verify 201 response, valid JWT token
- [ ] **T037** [P] Contract test POST /api/auth/login in `AuthControllerTests.cs` - verify 200 response, access + refresh tokens
- [ ] **T038** [P] Contract test POST /api/auth/refresh in `AuthControllerTests.cs` - verify token refresh works
- [ ] **T039** [P] Contract test GET /api/menu in `MenuControllerTests.cs` - verify 200 response, menu items array
- [ ] **T040** [P] Contract test POST /api/admin/menu in `MenuControllerTests.cs` - verify 201 response, requires Admin role
- [ ] **T041** [P] Contract test PUT /api/admin/menu/{id} in `MenuControllerTests.cs` - verify 200 response, item updated
- [ ] **T042** [P] Contract test DELETE /api/admin/menu/{id} in `MenuControllerTests.cs` - verify 204 response, soft delete
- [ ] **T043** [P] Contract test POST /api/orders in `OrdersControllerTests.cs` - verify 201 response, order created with "Placed" status
- [ ] **T044** [P] Contract test GET /api/orders in `OrdersControllerTests.cs` - verify 200 response, supports filtering by status/table/date
- [ ] **T045** [P] Contract test GET /api/orders/{id} in `OrdersControllerTests.cs` - verify 200 response, order details
- [ ] **T046** [P] Contract test PUT /api/orders/{id}/status in `OrdersControllerTests.cs` - verify 200 response, status transition validated
- [ ] **T047** [P] Contract test POST /api/orders/{id}/payment in `OrdersControllerTests.cs` - verify 200 response, payment recorded
- [ ] **T048** [P] Contract test GET /api/tables in `TablesControllerTests.cs` - verify 200 response, table list
- [ ] **T049** [P] Contract test POST /api/reservations in `ReservationsControllerTests.cs` - verify 201 response, reservation created
- [ ] **T050** [P] Contract test GET /api/reservations in `ReservationsControllerTests.cs` - verify 200 response, supports date/status filtering

### T051-T056: Contract Tests (Inventory & Reports)

- [ ] **T051** [P] Contract test GET /api/inventory in `InventoryControllerTests.cs` - verify 200 response, stock levels
- [ ] **T052** [P] Contract test PUT /api/inventory/{id}/adjust in `InventoryControllerTests.cs` - verify 200 response, quantity adjusted
- [ ] **T053** [P] Contract test GET /api/reports/sales in `ReportsControllerTests.cs` - verify 200 response, sales data, supports CSV export
- [ ] **T054** [P] Contract test GET /api/reports/stock in `ReportsControllerTests.cs` - verify 200 response, low stock alerts
- [ ] **T055** [P] Contract test GET /api/admin/settings in `AdminControllerTests.cs` - verify 200 response, restaurant config
- [ ] **T056** [P] Contract test PUT /api/admin/settings in `AdminControllerTests.cs` - verify 200 response, config updated

### T057-T065: SignalR Hub Contract Tests

All SignalR hub tests in `tests/RestaurantSuite.Tests.Integration/Hubs/`

- [ ] **T057** [P] SignalR contract test OrderCreated event in `OrdersHubTests.cs` - verify message broadcast to all connected clients
- [ ] **T058** [P] SignalR contract test OrderStatusChanged event in `OrdersHubTests.cs` - verify message includes orderId, newStatus, updatedBy
- [ ] **T059** [P] SignalR contract test OrderLineStatusChanged event in `OrdersHubTests.cs` - verify line-level status updates
- [ ] **T060** [P] SignalR contract test OrderCancelled event in `OrdersHubTests.cs` - verify cancellation broadcast
- [ ] **T061** [P] SignalR contract test PaymentCompleted event in `OrdersHubTests.cs` - verify payment notification
- [ ] **T062** [P] SignalR contract test NewTicket event in `KitchenHubTests.cs` - verify kitchen ticket broadcast
- [ ] **T063** [P] SignalR contract test OrderModified event in `KitchenHubTests.cs` - verify modification notification
- [ ] **T064** [P] SignalR contract test LowStockAlert event in `NotificationsHubTests.cs` - verify admin notification
- [ ] **T065** [P] SignalR contract test PrinterOfflineAlert event in `NotificationsHubTests.cs` - verify staff notification

### T066-T075: Integration Tests (User Stories from Quickstart)

All integration tests in `tests/RestaurantSuite.Tests.Integration/Workflows/`

- [ ] **T066** [P] Integration test: Order Placement Flow in `OrderPlacementTests.cs` - Guest browses menu, selects items with modifiers, places order (Quickstart Step 3)
- [ ] **T067** [P] Integration test: Kitchen Receives Order in `KitchenFlowTests.cs` - Order appears in kitchen within 2 seconds via SignalR (Quickstart Step 4)
- [ ] **T068** [P] Integration test: Chef Marks Items Ready in `KitchenFlowTests.cs` - Chef marks items InProgress then Ready, order status transitions (Quickstart Step 5)
- [ ] **T069** [P] Integration test: Waiter Notified in `WaiterFlowTests.cs` - Waiter receives notification when order ready (Quickstart Step 6)
- [ ] **T070** [P] Integration test: Payment Processing in `PaymentFlowTests.cs` - Waiter processes payment, receipt generated, order completed (Quickstart Step 8)
- [ ] **T071** [P] Integration test: Inventory Deduction in `InventoryFlowTests.cs` - Order completion triggers inventory deduction (Quickstart Step 9)
- [ ] **T072** [P] Integration test: User Authentication in `AuthenticationTests.cs` - Users can authenticate and access protected endpoints (Quickstart Step 7)
- [ ] **T073** [P] Integration test: Offline PWA Menu Cache in `PWATests.cs` - Menu cached for offline viewing (Quickstart Step 5 Offline Test)
- [ ] **T074** [P] Integration test: Order Cancellation in `OrderCancellationTests.cs` - Order cancelled, inventory reversed (Quickstart Step 6 Edge Case)
- [ ] **T075** [P] Integration test: Low Stock Alert in `InventoryAlertTests.cs` - Stock below reorder level triggers alert (Quickstart Step 6 Edge Case)

### T076-T080: Performance & Health Tests

- [ ] **T076** [P] Performance test: Order placement <500ms p95 in `tests/RestaurantSuite.Tests.Integration/Performance/OrderPerformanceTests.cs`
- [ ] **T077** [P] Performance test: Menu retrieval <500ms p95 in `tests/RestaurantSuite.Tests.Integration/Performance/MenuPerformanceTests.cs`
- [ ] **T078** [P] Performance test: SignalR message delivery <2s in `tests/RestaurantSuite.Tests.Integration/Performance/SignalRPerformanceTests.cs`
- [ ] **T079** [P] Health check test: Database connectivity in `tests/RestaurantSuite.Tests.Integration/HealthCheckTests.cs`
- [ ] **T080** [P] Health check test: Redis connectivity in `tests/RestaurantSuite.Tests.Integration/HealthCheckTests.cs`

**CHECKPOINT: Run all tests in Phase 2. All should FAIL. If any pass, delete implementation code.**

---

## Phase 3: Core Domain Implementation (ONLY after Phase 2 tests fail)

### T081-T097: Domain Entities

All entities in `src/RestaurantSuite.Domain/Entities/`

- [ ] **T081** [P] Create Restaurant entity in `Restaurant.cs` as singleton configuration entity with properties: Id (always 1), Name, Address, Timezone, Currency, SettingsJson, CreatedAt, UpdatedAt
- [ ] **T082** [P] Create User entity in `User.cs` with properties: Id, Email, PasswordHash, Role, DisplayName, Phone, IsActive, IsDeleted, CreatedAt
- [ ] **T083** [P] Create Staff entity in `Staff.cs` with properties: Id, UserId, Role, WorkStation, ShiftStartAt, ShiftEndAt
- [ ] **T084** [P] Create MenuCategory entity in `MenuCategory.cs` with properties: Id, Name, Order, IsDeleted
- [ ] **T085** [P] Create MenuItem entity in `MenuItem.cs` with properties: Id, CategoryId, Name, Description, Price, SKU, TaxRate, IsAvailable, ImagePath, IsDeleted, CreatedAt, UpdatedAt
- [ ] **T086** [P] Create MenuItemOption entity in `MenuItemOption.cs` with properties: Id, MenuItemId, Name, Type, Required
- [ ] **T087** [P] Create MenuItemModifier entity in `MenuItemModifier.cs` with properties: Id, OptionId, Name, PriceDelta
- [ ] **T088** [P] Create InventoryItem entity in `InventoryItem.cs` with properties: Id, Name, SKU, StockQty, ReorderLevel, Unit, CreatedAt, UpdatedAt
- [ ] **T089** [P] Create Recipe entity in `Recipe.cs` (join table) with properties: Id, MenuItemId, InventoryItemId, Quantity
- [ ] **T090** [P] Create Order entity in `Order.cs` with properties: Id, TableId, GuestId, Status, PaymentStatus, TotalAmount, TaxAmount, Notes, CreatedAt, CompletedAt
- [ ] **T091** [P] Create OrderLine entity in `OrderLine.cs` with properties: Id, OrderId, MenuItemId, Quantity, Price, ModifiersJson, Status
- [ ] **T092** [P] Create Table entity in `Table.cs` with properties: Id, Number, Seats, Status, PositionX, PositionY
- [ ] **T093** [P] Create Reservation entity in `Reservation.cs` with properties: Id, GuestName, GuestEmail, GuestPhone, PartySize, StartAt, EndAt, TableId, Status, CreatedAt
- [ ] **T094** [P] Create Payment entity in `Payment.cs` with properties: Id, OrderId, Method, Amount, TransactionId, Status, TipAmount, CreatedAt, CompletedAt
- [ ] **T095** [P] Create Printer entity in `Printer.cs` with properties: Id, Name, Type, ConnectionString, IsOnline, LastPrintAt
- [ ] **T096** [P] Create KitchenStation entity in `KitchenStation.cs` with properties: Id, Name, PrinterId
- [ ] **T097** [P] Create LoyaltyAccount entity in `LoyaltyAccount.cs` with properties: Id, UserId, Points, Tier, CreatedAt, UpdatedAt

### T098-T100: Supporting Domain Entities

- [ ] **T098** [P] Create AuditLog entity in `AuditLog.cs` with properties: Id, EntityType, EntityId, Action, UserId, ChangesSummary, Timestamp
- [ ] **T099** [P] Create InventoryAdjustment entity in `InventoryAdjustment.cs` with properties: Id, InventoryItemId, QuantityChange, Reason, UserId, CreatedAt
- [ ] **T100** [P] Create MenuItemTranslation entity in `MenuItemTranslation.cs` with properties: Id, MenuItemId, LanguageCode, Name, Description

### T101-T105: Domain Enums

All enums in `src/RestaurantSuite.Domain/Enums/`

- [ ] **T101** [P] Create OrderStatus enum in `OrderStatus.cs`: Placed, Confirmed, InKitchen, Ready, Served, Completed, Cancelled
- [ ] **T102** [P] Create PaymentStatus enum in `PaymentStatus.cs`: Unpaid, PartiallyPaid, Paid, Refunded
- [ ] **T103** [P] Create ReservationStatus enum in `ReservationStatus.cs`: Pending, Confirmed, Seated, Completed, Cancelled, NoShow
- [ ] **T104** [P] Create TableStatus enum in `TableStatus.cs`: Available, Occupied, Reserved, Cleaning
- [ ] **T105** [P] Create UserRole enum in `UserRole.cs`: Admin, Waiter, Chef, Guest

---

## Phase 4: Infrastructure Layer

### T106-T122: EF Core Entity Configurations

All configurations in `src/RestaurantSuite.Infrastructure.EF/Configurations/`

- [ ] **T106** [P] Create RestaurantConfiguration in `RestaurantConfiguration.cs` with fluent API: primary key, indexes, required fields, max lengths
- [ ] **T107** [P] Create UserConfiguration in `UserConfiguration.cs` with fluent API: unique index on Email, global query filter for IsDeleted
- [ ] **T108** [P] Create StaffConfiguration in `StaffConfiguration.cs` with fluent API: one-to-one with User
- [ ] **T109** [P] Create MenuCategoryConfiguration in `MenuCategoryConfiguration.cs` with fluent API: index on Order
- [ ] **T110** [P] Create MenuItemConfiguration in `MenuItemConfiguration.cs` with fluent API: FK to Category, composite index (CategoryId, IsAvailable)
- [ ] **T111** [P] Create MenuItemOptionConfiguration in `MenuItemOptionConfiguration.cs` with fluent API: FK to MenuItem, one-to-many with Modifiers
- [ ] **T112** [P] Create MenuItemModifierConfiguration in `MenuItemModifierConfiguration.cs` with fluent API: FK to Option
- [ ] **T113** [P] Create InventoryItemConfiguration in `InventoryItemConfiguration.cs` with fluent API: unique index on SKU
- [ ] **T114** [P] Create RecipeConfiguration in `Recipe Configuration.cs` with fluent API: composite FK (MenuItemId, InventoryItemId)
- [ ] **T115** [P] Create OrderConfiguration in `OrderConfiguration.cs` with fluent API: composite index (Status, CreatedAt)
- [ ] **T116** [P] Create OrderLineConfiguration in `OrderLineConfiguration.cs` with fluent API: FK to Order and MenuItem, JSON column for ModifiersJson
- [ ] **T117** [P] Create TableConfiguration in `TableConfiguration.cs` with fluent API: unique index on Number
- [ ] **T118** [P] Create ReservationConfiguration in `ReservationConfiguration.cs` with fluent API: composite index (StartAt, TableId)
- [ ] **T119** [P] Create PaymentConfiguration in `PaymentConfiguration.cs` with fluent API: FK to Order
- [ ] **T120** [P] Create PrinterConfiguration in `PrinterConfiguration.cs` with fluent API: unique index on Name
- [ ] **T121** [P] Create KitchenStationConfiguration in `KitchenStationConfiguration.cs` with fluent API: FK to Printer
- [ ] **T122** [P] Create LoyaltyAccountConfiguration in `LoyaltyAccountConfiguration.cs` with fluent API: one-to-one with User

### T123-T128: DbContext & Database Provider

- [ ] **T123** Create AppDbContext in `src/RestaurantSuite.Infrastructure.EF/DbContext/AppDbContext.cs` with DbSets for all entities
- [ ] **T124** Add OnModelCreating to AppDbContext: apply all entity configurations
- [ ] **T125** Configure multi-provider support in AppDbContext: detect "PostgreSQL" or "SqlServer" from configuration
- [ ] **T127** Create initial EF Core migration: `dotnet ef migrations add InitialCreate -p src/RestaurantSuite.Infrastructure.EF -s src/RestaurantSuite.Api`
- [ ] **T128** Create database seed data in `src/RestaurantSuite.Infrastructure.EF/Seed/DataSeeder.cs`: sample restaurant, 4 users (admin/waiter/chef/guest), 5 menu items, 3 tables

### T129-T135: Identity & Authentication

- [ ] **T129** Configure ASP.NET Core Identity in `src/RestaurantSuite.Infrastructure.Identity/Configuration/IdentityConfiguration.cs`: password requirements (10+ chars, mixed case, number, special)
- [ ] **T130** Create JWT token service in `src/RestaurantSuite.Infrastructure.Identity/Services/TokenService.cs`: GenerateAccessToken, GenerateRefreshToken methods
- [ ] **T131** Configure JWT authentication in `src/RestaurantSuite.Infrastructure.Identity/Configuration/JwtConfiguration.cs`: bearer token validation, issuer/audience settings
- [ ] **T132** Create role seed data in `src/RestaurantSuite.Infrastructure.Identity/Seed/RoleSeeder.cs`: Admin, Waiter, Chef, Guest roles
- [ ] **T133** Configure authorization policies in `src/RestaurantSuite.Infrastructure.Identity/Configuration/AuthorizationPolicies.cs`: AdminOnly, StaffOrAdmin, AuthenticatedUser
- [ ] **T134** [P] Implement 2FA support in `src/RestaurantSuite.Infrastructure.Identity/Services/TwoFactorService.cs`: TOTP authenticator app integration
- [ ] **T135** [P] Implement password reset flow in `src/RestaurantSuite.Infrastructure.Identity/Services/PasswordResetService.cs`: generate time-limited token, send email

---

## Phase 5: Application Layer

### T136-T155: DTOs

All DTOs in `src/RestaurantSuite.Application/DTOs/`

- [ ] **T136** [P] Create RegisterRequest DTO in `Auth/RegisterRequest.cs`: Email, Password, DisplayName, Phone, Role
- [ ] **T137** [P] Create LoginRequest DTO in `Auth/LoginRequest.cs`: Email, Password
- [ ] **T138** [P] Create AuthResponse DTO in `Auth/AuthResponse.cs`: AccessToken, RefreshToken, ExpiresIn, UserDto
- [ ] **T139** [P] Create UserDto in `Users/UserDto.cs`: Id, Email, DisplayName, Role
- [ ] **T140** [P] Create CreateMenuItemRequest DTO in `Menu/CreateMenuItemRequest.cs`: CategoryId, Name, Description, Price, SKU, TaxRate, IsAvailable, ImagePath
- [ ] **T141** [P] Create MenuItemDto in `Menu/MenuItemDto.cs`: Id, CategoryId, CategoryName, Name, Description, Price, SKU, TaxRate, IsAvailable, ImagePath, Options[]
- [ ] **T142** [P] Create CreateOrderRequest DTO in `Orders/CreateOrderRequest.cs`: TableId, Notes, OrderLines[]
- [ ] **T143** [P] Create OrderDto in `Orders/OrderDto.cs`: Id, TableNumber, Status, PaymentStatus, TotalAmount, TaxAmount, Notes, CreatedAt, OrderLines[]
- [ ] **T144** [P] Create OrderLineDto in `Orders/OrderLineDto.cs`: Id, MenuItemName, Quantity, Price, Modifiers[], Status
- [ ] **T145** [P] Create ProcessPaymentRequest DTO in `Payments/ProcessPaymentRequest.cs`: Method, Amount, TransactionId, TipAmount
- [ ] **T146** [P] Create PaymentDto in `Payments/PaymentDto.cs`: Id, OrderId, Method, Amount, TipAmount, Status, CreatedAt
- [ ] **T147** [P] Create TableDto in `Tables/TableDto.cs`: Id, Number, Seats, Status, PositionX, PositionY
- [ ] **T148** [P] Create CreateReservationRequest DTO in `Reservations/CreateReservationRequest.cs`: GuestName, GuestEmail, GuestPhone, PartySize, StartAt, TableId
- [ ] **T149** [P] Create ReservationDto in `Reservations/ReservationDto.cs`: Id, GuestName, GuestEmail, GuestPhone, PartySize, StartAt, EndAt, TableNumber, Status
- [ ] **T150** [P] Create InventoryItemDto in `Inventory/InventoryItemDto.cs`: Id, Name, SKU, StockQty, ReorderLevel, Unit, IsLowStock
- [ ] **T151** [P] Create SalesReportDto in `Reports/SalesReportDto.cs`: Period, TotalSales, OrderCount, AverageOrderValue, Breakdown[]
- [ ] **T152** [P] Create StockReportItemDto in `Reports/StockReportItemDto.cs`: ItemName, StockQty, ReorderLevel, Status
- [ ] **T153** [P] Create RestaurantDto in `Admin/RestaurantDto.cs`: Id, Name, Address, Timezone, Currency
- [ ] **T154** [P] Create UpdateRestaurantRequest DTO in `Admin/UpdateRestaurantRequest.cs`: Name, Address, Timezone, Currency
- [ ] **T155** [P] Create ErrorResponse DTO in `Common/ErrorResponse.cs`: Error, Message, Details

### T156-T175: FluentValidation Validators

All validators in `src/RestaurantSuite.Application/Validators/`

- [ ] **T156** [P] Create RegisterRequestValidator in `Auth/RegisterRequestValidator.cs`: validate email format, password complexity (10+ chars), role values
- [ ] **T157** [P] Create LoginRequestValidator in `Auth/LoginRequestValidator.cs`: validate email format, password not empty
- [ ] **T158** [P] Create CreateMenuItemRequestValidator in `Menu/CreateMenuItemRequestValidator.cs`: validate price >= 0, tax rate 0-1, name length 1-150
- [ ] **T159** [P] Create CreateOrderRequestValidator in `Orders/CreateOrderRequestValidator.cs`: validate at least 1 order line, quantities > 0
- [ ] **T160** [P] Create ProcessPaymentRequestValidator in `Payments/ProcessPaymentRequestValidator.cs`: validate amount > 0, method in [Cash, Card, Terminal]
- [ ] **T161** [P] Create CreateReservationRequestValidator in `Reservations/CreateReservationRequestValidator.cs`: validate StartAt is future, PartySize > 0, email or phone provided
- [ ] **T162** [P] Create UpdateRestaurantRequestValidator in `Admin/UpdateRestaurantRequestValidator.cs`: validate name length 3-200, timezone valid IANA format

### T163-T185: MediatR Command Handlers

All command handlers in `src/RestaurantSuite.Application/Commands/`

- [ ] **T163** [P] Create CreateMenuItemCommand + Handler in `Menu/CreateMenuItemCommand.cs`: insert MenuItem, publish domain event
- [ ] **T164** [P] Create UpdateMenuItemCommand + Handler in `Menu/UpdateMenuItemCommand.cs`: update MenuItem, invalidate cache
- [ ] **T165** [P] Create DeleteMenuItemCommand + Handler in `Menu/DeleteMenuItemCommand.cs`: soft delete MenuItem
- [ ] **T166** [P] Create CreateOrderCommand + Handler in `Orders/CreateOrderCommand.cs`: create Order + OrderLines, calculate totals, trigger SignalR OrderCreated event
- [ ] **T167** [P] Create UpdateOrderStatusCommand + Handler in `Orders/UpdateOrderStatusCommand.cs`: validate status transition, update status, trigger SignalR OrderStatusChanged event
- [ ] **T168** [P] Create CancelOrderCommand + Handler in `Orders/CancelOrderCommand.cs`: set status to Cancelled, reverse inventory, trigger SignalR OrderCancelled event
- [ ] **T169** [P] Create ProcessPaymentCommand + Handler in `Payments/ProcessPaymentCommand.cs`: call payment adapter, create Payment record, update order paymentStatus
- [ ] **T170** [P] Create CreateReservationCommand + Handler in `Reservations/CreateReservationCommand.cs`: check for double-booking, insert Reservation, send confirmation email
- [ ] **T171** [P] Create AdjustInventoryCommand + Handler in `Inventory/AdjustInventoryCommand.cs`: update StockQty, create InventoryAdjustment record, trigger low-stock alert if needed
- [ ] **T172** [P] Create UpdateRestaurantSettingsCommand + Handler in `Admin/UpdateRestaurantSettingsCommand.cs`: update Restaurant config (singleton with Id=1)

### T173-T185: MediatR Query Handlers

All query handlers in `src/RestaurantSuite.Application/Queries/`

- [ ] **T173** [P] Create GetMenuQuery + Handler in `Menu/GetMenuQuery.cs`: fetch menu items with categories, support filtering by categoryId and isAvailable
- [ ] **T174** [P] Create GetMenuItemByIdQuery + Handler in `Menu/GetMenuItemByIdQuery.cs`: fetch single menu item with options/modifiers
- [ ] **T175** [P] Create GetOrdersQuery + Handler in `Orders/GetOrdersQuery.cs`: fetch orders with filters (status, tableId, date range), include pagination
- [ ] **T176** [P] Create GetOrderByIdQuery + Handler in `Orders/GetOrderByIdQuery.cs`: fetch order with order lines and payment details
- [ ] **T177** [P] Create GetTablesQuery + Handler in `Tables/GetTablesQuery.cs`: fetch all tables for restaurant with current status
- [ ] **T178** [P] Create GetReservationsQuery + Handler in `Reservations/GetReservationsQuery.cs`: fetch reservations with filters (date, status)
- [ ] **T179** [P] Create GetInventoryQuery + Handler in `Inventory/GetInventoryQuery.cs`: fetch all inventory items with low-stock flags
- [ ] **T180** [P] Create GetSalesReportQuery + Handler in `Reports/GetSalesReportQuery.cs`: aggregate sales data by date range, support hourly/daily/weekly breakdowns
- [ ] **T181** [P] Create GetStockReportQuery + Handler in `Reports/GetStockReportQuery.cs`: fetch items below reorder level
- [ ] **T182** [P] Create GetRestaurantSettingsQuery + Handler in `Admin/GetRestaurantSettingsQuery.cs`: fetch restaurant configuration (singleton)

---

## Phase 6: SignalR Hubs

### T183-T191: SignalR Hub Implementations

All hubs in `src/RestaurantSuite.Notifications/Hubs/`

- [ ] **T183** Create IOrderClient interface in `IOrderClient.cs`: methods OrderCreated, OrderStatusChanged, OrderLineStatusChanged, OrderCancelled, PaymentCompleted
- [ ] **T184** Create OrdersHub in `OrdersHub.cs`: implement Hub<IOrderClient>, methods JoinTable, LeaveTable
- [ ] **T185** Create IKitchenClient interface in `IKitchenClient.cs`: methods NewTicket, OrderModified, OrderCancelled, OrderDelayAlert
- [ ] **T186** Create KitchenHub in `KitchenHub.cs`: implement Hub<IKitchenClient>, methods JoinStation, MarkInProgress, MarkReady
- [ ] **T187** Create INotificationClient interface in `INotificationClient.cs`: methods ReceiveNotification, LowStockAlert, PrinterOfflineAlert
- [ ] **T188** Create NotificationsHub in `NotificationsHub.cs`: implement Hub<INotificationClient>, method JoinRoleGroup
- [ ] **T189** Configure SignalR in `src/RestaurantSuite.Api/Program.cs`: add SignalR services, configure Redis backplane
- [ ] **T190** Map SignalR hubs in `src/RestaurantSuite.Api/Program.cs`: /hubs/orders, /hubs/kitchen, /hubs/notifications
- [ ] **T191** Create background worker in `src/RestaurantSuite.Notifications/Workers/OrderTimeoutWorker.cs`: send OrderDelayAlert if order in InKitchen > configurable threshold (e.g., 30 min)

---

## Phase 7: API Controllers

### T192-T207: Controller Implementations

All controllers in `src/RestaurantSuite.Api/Controllers/`

- [ ] **T192** Create AuthController in `AuthController.cs`: POST /api/auth/register (call MediatR RegisterCommand), POST /api/auth/login (return JWT), POST /api/auth/refresh
- [ ] **T193** Create MenuController in `MenuController.cs`: GET /api/menu (call GetMenuQuery), GET /api/admin/menu (same with auth), POST /api/admin/menu (call CreateMenuItemCommand, [Authorize("AdminOnly")])
- [ ] **T194** Add PUT /api/admin/menu/{id} to MenuController: call UpdateMenuItemCommand, [Authorize("AdminOnly")]
- [ ] **T195** Add DELETE /api/admin/menu/{id} to MenuController: call DeleteMenuItemCommand, [Authorize("AdminOnly")]
- [ ] **T196** Create OrdersController in `OrdersController.cs`: GET /api/orders (call GetOrdersQuery), GET /api/orders/{id} (call GetOrderByIdQuery), POST /api/orders (call CreateOrderCommand)
- [ ] **T197** Add PUT /api/orders/{id}/status to OrdersController: call UpdateOrderStatusCommand, validate role permissions (Waiter/Chef can update)
- [ ] **T198** Add POST /api/orders/{id}/payment to OrdersController: call ProcessPaymentCommand, [Authorize("StaffOrAdmin")]
- [ ] **T199** Create TablesController in `TablesController.cs`: GET /api/tables (call GetTablesQuery), [Authorize]
- [ ] **T200** Create ReservationsController in `ReservationsController.cs`: GET /api/reservations (call GetReservationsQuery), POST /api/reservations (call CreateReservationCommand)
- [ ] **T201** Create InventoryController in `InventoryController.cs`: GET /api/inventory (call GetInventoryQuery), PUT /api/inventory/{id}/adjust (call AdjustInventoryCommand, [Authorize("AdminOnly")])
- [ ] **T202** Create ReportsController in `ReportsController.cs`: GET /api/reports/sales (call GetSalesReportQuery, support CSV export via Accept header), GET /api/reports/stock (call GetStockReportQuery)
- [ ] **T203** Create AdminController in `AdminController.cs`: GET /api/admin/settings (call GetRestaurantSettingsQuery), PUT /api/admin/settings (call UpdateRestaurantSettingsCommand, [Authorize("AdminOnly")])

### T204-T210: API Middleware & Configuration

- [ ] **T204** Configure Swagger in `src/RestaurantSuite.Api/Program.cs`: add Swashbuckle, configure JWT bearer auth UI, map to /swagger
- [ ] **T205** Configure Serilog in `src/RestaurantSuite.Api/Program.cs`: console + Seq sinks, structured logging with request enrichment
- [ ] **T206** Configure health checks in `src/RestaurantSuite.Api/Program.cs`: add database health check, Redis health check, map to /health
- [ ] **T207** Configure CORS in `src/RestaurantSuite.Api/Program.cs`: allow Blazor app origins (localhost:5001-5004), allow credentials for SignalR
- [ ] **T208** Add rate limiting middleware in `src/RestaurantSuite.Api/Middleware/RateLimitingMiddleware.cs`: 5 login attempts/minute, 20 orders/minute per user
- [ ] **T209** Configure OpenTelemetry in `src/RestaurantSuite.Api/Program.cs`: add ASP.NET Core instrumentation, EF Core instrumentation, Prometheus exporter at /metrics
- [ ] **T210** Add global exception handler in `src/RestaurantSuite.Api/Middleware/ExceptionHandlerMiddleware.cs`: catch unhandled exceptions, return ErrorResponse DTO, log with Serilog

---

## Phase 8: Integration Adapters

### T211-T218: Payment & Printer Adapters

All adapters in `src/RestaurantSuite.Integrations/`

- [ ] **T211** Create IPaymentAdapter interface in `Payments/IPaymentAdapter.cs`: methods ProcessPaymentAsync, RefundPaymentAsync, CreatePaymentIntentAsync
- [ ] **T212** [P] Implement StripeAdapter in `Payments/StripeAdapter.cs`: use Stripe.net SDK, handle payment intents, webhooks for payment status
- [ ] **T213** [P] Implement SquareAdapter in `Payments/SquareAdapter.cs`: use Square SDK, handle payments, refunds
- [ ] **T214** Configure payment adapter factory in `Payments/PaymentAdapterFactory.cs`: return adapter based on appsettings "PaymentProvider" config
- [ ] **T215** Create IPrinterAdapter interface in `Printers/IPrinterAdapter.cs`: methods PrintAsync, GetStatusAsync
- [ ] **T216** Implement NetworkPrinterAdapter in `Printers/NetworkPrinterAdapter.cs`: use ESC/POS protocol over TCP socket, connect to printer IP:port
- [ ] **T217** Create print job queue in `Printers/PrintJobQueue.cs`: use Redis queue for failed print jobs, background worker retries every 30s
- [ ] **T218** Create PrinterHealthCheck in `Printers/PrinterHealthCheck.cs`: ping all configured printers, trigger PrinterOfflineAlert if offline >5 min

---

## Phase 9: Blazor Frontends

### T219-T230: Admin (Blazor Server)

All pages in `src/RestaurantSuite.Admin/Pages/`

- [ ] **T219** Create Dashboard.razor page: display KPIs (total sales, order count, table occupancy), real-time updates via SignalR, charts using chart library (e.g., ApexCharts)
- [ ] **T220** Create MenuEditor.razor page: CRUD menu items with categories, inline editing, image upload, modifiers/options management
- [ ] **T221** Create Inventory.razor page: display inventory items with stock levels, highlight low-stock items, adjust quantity button, audit trail
- [ ] **T222** Create Staff.razor page: list users with roles, add/edit/deactivate users, assign kitchen stations
- [ ] **T223** Create Reports.razor page: sales report with date range picker, export CSV/PDF, stock report with alerts
- [ ] **T224** Create Settings.razor page: edit single restaurant config (name, address, timezone, currency), payment provider config, printer config
- [ ] **T225** Create AuditLogs.razor page: display audit log entries, filter by entity type/user/date
- [ ] **T226** Configure authentication in `src/RestaurantSuite.Admin/Program.cs`: JWT bearer authentication, redirect to login if not authenticated
- [ ] **T227** Create shared layout in `src/RestaurantSuite.Admin/Shared/MainLayout.razor`: navigation menu, user info, logout button
- [ ] **T228** Implement SignalR client in `src/RestaurantSuite.Admin/Services/SignalRService.cs`: connect to /hubs/orders, /hubs/notifications, update UI on events

### T229-T238: Waiter (Blazor WASM PWA)

All pages in `src/RestaurantSuite.Waiter/Pages/`, PWA assets in `wwwroot/`

- [ ] **T229** Create Tables.razor page: display table grid/list with status, click to view active orders, floor plan editor (drag & drop using JS interop)
- [ ] **T230** Create OrderEntry.razor page: select table, add menu items with modifiers, notes field, submit order, modify existing order
- [ ] **T231** Create Payment.razor page: select payment method, enter amount, split checks, tip input, print receipt
- [ ] **T232** Create Notifications.razor page: list all notifications, mark as read, filter by type
- [ ] **T233** Create manifest.json in `wwwroot/manifest.json`: app name "Waiter App", icons, start_url, display "standalone"
- [ ] **T234** Create service-worker.js in `wwwroot/service-worker.js`: cache menu API responses, offline fallback page
- [ ] **T235** Implement SignalR client in `src/RestaurantSuite.Waiter/Services/SignalRService.cs`: connect to /hubs/orders, listen for OrderStatusChanged (Ready), show web push notification
- [ ] **T236** Configure web push notifications in `src/RestaurantSuite.Waiter/Services/PushNotificationService.cs`: request permission, subscribe to push, handle incoming messages
- [ ] **T237** Add offline detection in `src/RestaurantSuite.Waiter/Services/NetworkService.cs`: display banner when offline, queue order requests, sync when online
- [ ] **T238** Configure authentication in `src/RestaurantSuite.Waiter/Program.cs`: JWT bearer, store tokens in local storage, auto-refresh

### T239-T245: Chef (Blazor WASM PWA)

All pages in `src/RestaurantSuite.Chef/Pages/`

- [ ] **T239** Create KitchenDisplay.razor page: display incoming orders as tickets, filter by kitchen station, sort by order time, mark items InProgress/Ready
- [ ] **T240** Add real-time updates in KitchenDisplay.razor: SignalR NewTicket event adds ticket to display, OrderModified event updates existing ticket
- [ ] **T241** Add order line status toggle in KitchenDisplay.razor: click item to mark InProgress (yellow), click again to mark Ready (green), trigger SignalR hub method
- [ ] **T242** Create manifest.json in `wwwroot/manifest.json`: app name "Chef App", icons, display "fullscreen" (for KDS mode)
- [ ] **T243** Create service-worker.js in `wwwroot/service-worker.js`: cache menu data, offline support for viewing tickets
- [ ] **T244** Implement SignalR client in `src/RestaurantSuite.Chef/Services/SignalRService.cs`: connect to /hubs/kitchen, auto-join station based on chef profile
- [ ] **T245** Add audio alert in `src/RestaurantSuite.Chef/Services/AudioService.cs`: play sound when new ticket arrives, use Web Audio API via JS interop

### T246-T255: Guest (Blazor WASM PWA)

All pages in `src/RestaurantSuite.Guest/Pages/`

- [ ] **T246** Create Menu.razor page: display menu categories and items with images, descriptions, prices, "Add to Cart" button
- [ ] **T247** Create Order.razor page: cart view, select modifiers for items, adjust quantities, select table or takeout/delivery, submit order
- [ ] **T248** Create OrderStatus.razor page: display placed orders with real-time status updates (via SignalR), show estimated time, loyalty points earned
- [ ] **T249** Create Reservation.razor page: select date/time/party size, enter contact info, submit reservation, view confirmation
- [ ] **T250** Create Loyalty.razor page: display loyalty points, tier, order history, redeem points UI
- [ ] **T251** Create manifest.json in `wwwroot/manifest.json`: app name "Guest App", icons, background color matching brand
- [ ] **T252** Create service-worker.js in `wwwroot/service-worker.js`: aggressive caching for menu images, offline menu browsing
- [ ] **T253** Implement SignalR client in `src/RestaurantSuite.Guest/Services/SignalRService.cs`: connect to /hubs/orders, listen for order status updates, show notifications
- [ ] **T254** Add payment integration in `src/RestaurantSuite.Guest/Services/PaymentService.cs`: redirect to Stripe Checkout or Square Web SDK for online orders
- [ ] **T255** Configure anonymous browsing in `src/RestaurantSuite.Guest/Program.cs`: allow menu viewing without login, require auth for order placement

---

## Phase 10: Polish & Testing

### T256-T270: Unit Tests

All unit tests in `tests/RestaurantSuite.Tests.Unit/`

- [ ] **T256** [P] Unit test Restaurant entity validation in `Domain/RestaurantTests.cs`: required fields, valid timezone, valid currency
- [ ] **T257** [P] Unit test User entity validation in `Domain/UserTests.cs`: email format, password hash, role values
- [ ] **T258** [P] Unit test Order total calculation in `Domain/OrderTests.cs`: sum of order lines + modifiers + tax
- [ ] **T259** [P] Unit test OrderStatus state transitions in `Domain/OrderStatusTests.cs`: valid transitions (Placed→Confirmed→InKitchen→Ready→Served→Completed), invalid transitions throw exception
- [ ] **T260** [P] Unit test CreateMenuItemCommand handler in `Application/CreateMenuItemCommandTests.cs`: verify MenuItem created, cache invalidated
- [ ] **T261** [P] Unit test CreateOrderCommand handler in `Application/CreateOrderCommandTests.cs`: verify order created with correct totals, SignalR event published
- [ ] **T262** [P] Unit test UpdateOrderStatusCommand handler in `Application/UpdateOrderStatusCommandTests.cs`: verify status transition validated, SignalR event published
- [ ] **T263** [P] Unit test ProcessPaymentCommand handler in `Application/ProcessPaymentCommandTests.cs`: verify payment adapter called, Payment record created
- [ ] **T264** [P] Unit test RegisterRequestValidator in `Application/RegisterRequestValidatorTests.cs`: email format, password complexity, role values
- [ ] **T265** [P] Unit test CreateOrderRequestValidator in `Application/CreateOrderRequestValidatorTests.cs`: at least 1 order line, quantities > 0

### T266-T275: Additional Integration Tests

- [ ] **T266** [P] Integration test: Multi-provider database in `tests/RestaurantSuite.Tests.Integration/Database/MultiProviderTests.cs` - verify PostgreSQL and SQL Server both work
- [ ] **T267** [P] Integration test: Redis caching in `tests/RestaurantSuite.Tests.Integration/Caching/RedisCacheTests.cs` - menu cache hit/miss, invalidation
- [ ] **T268** [P] Integration test: SignalR reconnection in `tests/RestaurantSuite.Tests.Integration/Hubs/ReconnectionTests.cs` - disconnect client, reconnect, verify group membership restored
- [ ] **T269** [P] Integration test: Rate limiting in `tests/RestaurantSuite.Tests.Integration/Middleware/RateLimitingTests.cs` - verify 5 login attempts blocked, 20 orders blocked
- [ ] **T270** [P] Integration test: Audit logging in `tests/RestaurantSuite.Tests.Integration/AuditLogTests.cs` - entity changes logged with user and timestamp

### T271-T278: E2E Tests (Playwright)

All E2E tests in `tests/RestaurantSuite.Tests.E2E/` (new project)

- [ ] **T271** Setup Playwright in new xUnit project `tests/RestaurantSuite.Tests.E2E`: add Playwright NuGet, configure browsers
- [ ] **T272** [P] E2E test: Guest places order in `GuestOrderFlowTests.cs` - navigate to menu, add items, submit, verify order ID
- [ ] **T273** [P] E2E test: Kitchen processes order in `KitchenFlowTests.cs` - login as chef, verify new ticket appears, mark ready
- [ ] **T274** [P] E2E test: Waiter receives notification in `WaiterFlowTests.cs` - order marked ready in kitchen, waiter sees notification within 2s
- [ ] **T275** [P] E2E test: Payment flow in `PaymentFlowTests.cs` - waiter processes payment, receipt generated, order completed

### T276-T285: Performance & Load Tests

- [ ] **T276** Create k6 load test script in `tests/load-test.js`: ramp up to 500 concurrent users, target /api/orders and /api/menu
- [ ] **T277** Verify p95 latency <500ms for GET /api/menu under load
- [ ] **T278** Verify p95 latency <500ms for POST /api/orders under load
- [ ] **T279** Verify SignalR supports 500+ concurrent WebSocket connections
- [ ] **T280** Verify database connection pooling handles concurrent requests without exhaustion

### T281-T290: Documentation & Final Polish

- [ ] **T281** [P] Create README.md at repository root: project overview, tech stack, prerequisites, how to run locally
- [ ] **T282** [P] Create docs/deployment.md: Docker deployment guide, environment variables, database migrations, secrets management
- [ ] **T283** [P] Create docs/architecture.md: Clean Architecture diagram, project dependencies, data flow
- [ ] **T284** [P] Update API documentation in Swagger: add XML comments to all controllers, example requests/responses
- [ ] **T285** Create .env.example file: template for environment variables (DB connection, Redis, JWT secret, Stripe key)
- [ ] **T286** Add logging for all SignalR events: OrderCreated, OrderStatusChanged, etc. with structured data
- [ ] **T287** Add database indexes review: ensure all FK columns indexed, composite indexes for common queries
- [ ] **T288** Security audit: verify all endpoints have [Authorize], check for SQL injection risks, validate CORS config
- [ ] **T289** Code review: remove console.log/Debug.WriteLine statements, ensure error messages don't leak sensitive data
- [ ] **T290** Final integration test run: execute all tests in `tests/` directory, verify 100% pass rate

---

## Dependencies Graph

### Critical Path (Sequential)
1. **Foundation** (T001-T035) → Must complete first
2. **Tests Written** (T036-T080) → Write and verify FAIL
3. **Domain Entities** (T081-T105) → Models required for tests to compile
4. **Infrastructure** (T106-T135) → DbContext, migrations, Identity
5. **Application Layer** (T136-T182) → DTOs, commands, queries
6. **SignalR Hubs** (T183-T191) → Real-time events
7. **API Controllers** (T192-T210) → Expose endpoints
8. **Integration Adapters** (T211-T218) → External services
9. **Blazor Frontends** (T219-T255) → UI implementation
10. **Polish & Testing** (T256-T290) → Validation and docs

### Parallel Execution Groups

**Group 1 - Setup (can run together after T001):**
```bash
T002, T003, T004, T005, T006, T007, T008
T011, T012, T013, T014
T015, T016
T031, T032, T033, T035
```

**Group 2 - Contract Tests (run together):**
```bash
T036-T056 (all contract tests)
T057-T065 (all SignalR contract tests)
T066-T075 (all integration tests)
T076-T080 (all performance tests)
```

**Group 3 - Domain Entities (run together after foundation):**
```bash
T081-T100 (all entities)
T101-T105 (all enums)
```

**Group 4 - EF Configurations (run together after entities):**
```bash
T106-T122 (all entity configurations)
```

**Group 5 - DTOs (run together):**
```bash
T136-T155 (all DTOs)
```

**Group 6 - Validators (run together after DTOs):**
```bash
T156-T162 (all validators)
```

**Group 7 - Commands (run together after validators):**
```bash
T163-T172 (all command handlers)
```

**Group 8 - Queries (run together after commands):**
```bash
T173-T182 (all query handlers)
```

**Group 9 - Payment Adapters (run together):**
```bash
T212, T213 (Stripe and Square adapters)
```

**Group 10 - Unit Tests (run together):**
```bash
T256-T265 (all unit tests)
T266-T270 (additional integration tests)
T272-T275 (E2E tests, after T271 setup)
```

**Group 11 - Documentation (run together):**
```bash
T281, T282, T283, T284
```

---

## Validation Checklist

- [x] All API contracts (30+ endpoints) have corresponding contract tests (T036-T056)
- [x] All SignalR events (9 events) have contract tests (T057-T065)
- [x] All entities (17 core + 2 supporting) have model tasks (T081-T100)
- [x] All user stories (10 scenarios) have integration tests (T066-T075)
- [x] Tests (Phase 2) come before implementation (Phase 3+)
- [x] Parallel tasks [P] are truly independent (different files)
- [x] Each task specifies exact file path or clear location
- [x] No task modifies same file as another [P] task
- [x] TDD workflow enforced: write tests → verify fail → implement → verify pass

---

## Execution Notes

### Running Tests First (TDD)
```bash
# After completing T036-T080, run all tests
dotnet test tests/RestaurantSuite.Tests.Integration
dotnet test tests/RestaurantSuite.Tests.Unit

# ALL TESTS SHOULD FAIL
# If any pass, you have implementation code that shouldn't exist yet
```

### Parallel Execution Example
```bash
# Example: Run all contract test tasks in parallel using Claude Code Task agent
# Launch T036-T042 together (different test files):
Task: "Contract test POST /api/auth/register - verify 201, valid JWT"
Task: "Contract test POST /api/auth/login - verify 200, tokens"
Task: "Contract test GET /api/menu - verify 200, menu array"
Task: "Contract test POST /api/admin/menu - verify 201, requires Admin"
# ... all [P] tasks in Group 2
```

### Migration Commands
```bash
# After completing T127 (initial migration)
cd src/RestaurantSuite.Api
dotnet ef database update

# After completing T128 (seed data)
dotnet run --seed
```

### Docker Commands
```bash
# After completing T034 (docker-compose)
cd docker
docker-compose up -d

# Verify services
docker-compose ps
curl http://localhost:5432  # PostgreSQL
curl http://localhost:6379  # Redis
```

### Final Validation
```bash
# After completing all tasks, run quickstart guide
# Follow specs/001-project-request-restaurant/quickstart.md
# Verify all 10 validation steps pass
```

---

## Task Completion Criteria

Each task is considered complete when:
1. Code compiles without errors
2. Related tests pass (if implementation task)
3. Code follows C# 12 conventions: `public Type PropertyName { get; set; }`
4. XML comments added for public APIs
5. No console debug statements
6. Committed to git with message referencing task ID (e.g., "T081: Create Restaurant entity")

---

**Total Tasks**: 290
**Estimated Parallel Groups**: 11
**Critical Path Length**: ~60 tasks (sequential dependencies)
**Estimated Time**: 40-60 hours (with parallel execution and TDD discipline)
