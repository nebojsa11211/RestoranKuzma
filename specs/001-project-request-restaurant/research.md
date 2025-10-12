# Research & Technology Decisions

**Feature**: Restaurant Management & Ordering Suite
**Branch**: 001-project-request-restaurant
**Date**: 2025-10-06

## Overview
This document consolidates research findings and technology decisions for implementing a single-restaurant management system with real-time capabilities and offline support.

---

## Core Technology Stack

### .NET Version Selection

**Decision**: .NET 9 (STS - Standard Term Support)

**Rationale**:
- Latest features and performance improvements
- C# 12 support with collection expressions, primary constructors
- Native AOT improvements for Blazor WASM
- Enhanced minimal API features
- Better SignalR performance and scalability
- Improved EF Core 9 features (complex types, JSON columns)

**Alternatives Considered**:
- **.NET 8 (LTS)**: More stable for long-term production, but lacks latest performance optimizations
- **Recommendation**: Start with .NET 9 for development; can downgrade to .NET 8 LTS if stability issues arise

**Migration Path**: .NET 9 → .NET 8 is straightforward if needed (remove newer language features)

---

### Database Provider Strategy

**Decision**: Multi-provider support with PostgreSQL as primary

**Rationale**:
- PostgreSQL via Supabase for cloud deployments (built-in auth, real-time, storage)
- SQL Server for on-premise enterprise customers
- EF Core provider abstraction enables switching via configuration
- JSON column support in both (EF Core 8+)

**Implementation**:
```csharp
// Configuration-based provider selection
services.AddDbContext<AppDbContext>(options =>
{
    var provider = configuration["Database:Provider"]; // "PostgreSQL" or "SqlServer"
    if (provider == "PostgreSQL")
        options.UseNpgsql(connectionString);
    else
        options.UseSqlServer(connectionString);
});
```

**Best Practices**:
- Avoid database-specific SQL in migrations
- Use EF Core abstractions for JSON columns, full-text search
- Test migrations against both providers in CI

---

### Real-Time Communication

**Decision**: SignalR with Redis backplane

**Rationale**:
- Built-in .NET integration (no additional protocols to learn)
- Automatic fallback from WebSockets → Server-Sent Events → Long Polling
- Redis backplane enables horizontal scaling across multiple API instances
- Native reconnection handling
- Typed hubs for compile-time safety

**Alternatives Considered**:
- **Raw WebSockets**: More complex, no automatic reconnection
- **gRPC Streaming**: Overkill for broadcast scenarios
- **Server-Sent Events only**: Limited browser support for bidirectional

**Implementation Pattern**:
```csharp
// Hub registration with Redis scaleout
services.AddSignalR().AddStackExchangeRedis(redisConnectionString);

// Typed hub for order notifications
public interface IOrderClient
{
    Task OrderCreated(OrderDto order);
    Task OrderStatusChanged(Guid orderId, OrderStatus newStatus);
}

public class OrdersHub : Hub<IOrderClient>
{
    // Strongly-typed client proxy
}
```

**Performance Targets**:
- <2s notification delivery (per FR-085)
- Support 500+ concurrent WebSocket connections per instance
- Redis pub/sub for cross-instance messaging

---

### Authentication & Authorization

**Decision**: ASP.NET Core Identity + JWT with optional external providers

**Rationale**:
- ASP.NET Core Identity provides user/role management out-of-box
- JWT tokens for stateless API auth (mobile/PWA friendly)
- Refresh tokens for long-lived sessions (FR-005)
- Support OAuth 2.0/OIDC for Google, Apple (FR-009)
- Role-based + claim-based authorization

**Implementation**:
```csharp
// JWT configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* JWT validation */ })
    .AddGoogle(options => { /* OAuth */ })
    .AddOpenIdConnect("Supabase", options => { /* Supabase Auth */ });

// Role-based policies
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Admin", "Waiter", "Chef"));
});
```

**Security Practices**:
- Password complexity: 10+ chars, mixed case, numbers, symbols (FR-120)
- 2FA via TOTP (authenticator app) or SMS (FR-123)
- Rate limiting on auth endpoints: 5 attempts/minute (FR-121)
- Secure password reset with time-limited tokens (FR-008)
- HttpOnly cookies for refresh tokens to prevent XSS

---

### Blazor Architecture

**Decision**: Blazor Server for Admin, Blazor WASM for Waiter/Chef/Guest

**Rationale**:
- **Admin (Blazor Server)**: Real-time dashboard updates via SignalR already connected; no offline requirement
- **Waiter/Chef/Guest (Blazor WASM PWA)**: Offline menu viewing (FR-106), installable, web push notifications

**PWA Requirements** (FR-104):
- `manifest.json` for installability
- Service worker for caching and offline support
- Workbox or custom caching strategy for menu data
- Web Push API for notifications (FR-084)

**Caching Strategy**:
```javascript
// Service worker - cache menu images and data
self.addEventListener('fetch', event => {
    if (event.request.url.includes('/api/menu')) {
        event.respondWith(
            caches.match(event.request)
                .then(response => response || fetch(event.request))
        );
    }
});
```

**Trade-offs**:
- Blazor WASM: Larger initial download (~2MB), but offline-capable
- Blazor Server: Minimal download, but requires constant connection

---

### Payment Integration

**Decision**: Adapter pattern with Stripe and Square initial implementations

**Rationale**:
- Adapter pattern (Strategy) for swappable payment providers (FR-057)
- PCI DSS SAQ A compliance: Tokenization via provider SDKs, no raw card data stored
- Support both online (Stripe Checkout, Square Web SDK) and terminal payments

**Interface Design**:
```csharp
public interface IPaymentAdapter
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    Task<RefundResult> RefundPaymentAsync(string transactionId, decimal amount);
    Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency);
}

public class StripeAdapter : IPaymentAdapter { /* Stripe SDK */ }
public class SquareAdapter : IPaymentAdapter { /* Square SDK */ }
```

**Configuration**:
- Provider selection via appsettings: `"PaymentProvider": "Stripe"`
- Credentials stored in Azure Key Vault or environment variables

---

### Printer Integration

**Decision**: ESC/POS protocol over network (IP-based)

**Rationale**:
- ESC/POS is industry standard for receipt/kitchen printers
- Network printing avoids USB driver complications
- Queue and retry pattern for resilience (FR-078)

**Libraries**:
- `ESCPOS.NET` or `EscPosUtils` for command generation
- TCP socket connection to printer IP:port

**Implementation**:
```csharp
public interface IPrinterAdapter
{
    Task<bool> PrintAsync(PrintJob job);
    Task<PrinterStatus> GetStatusAsync();
}

public class NetworkPrinterAdapter : IPrinterAdapter
{
    // ESC/POS commands via TCP socket
}
```

**Queue Pattern**:
- Failed jobs stored in Redis or database
- Background worker retries every 30s
- Alert admin if printer offline >5 minutes (FR-079)

---

### Multi-Tenancy Architecture

## NOT APPLICABLE - Single Restaurant Architecture

**Decision**: Single-restaurant architecture for "Restoran Kuzma"

**Rationale**:
- This system is designed for a single restaurant ("Restoran Kuzma"), not a multi-tenant SaaS platform
- Eliminates complexity of tenant isolation and filtering
- Simplifies authentication and authorization model
- Reduces database index complexity and query overhead
- No need for tenant context middleware or global query filters

**Architecture Change Impact**:
This section was originally designed for multi-tenant architecture but has been simplified for single-restaurant deployment. The following multi-tenant features are NOT needed:

~~**Multi-Tenant Implementation** (NOT APPLICABLE):~~
```csharp
// REMOVED - No longer needed for single restaurant
// Global query filter for multi-tenancy
// modelBuilder.Entity<Order>().HasQueryFilter(o => o.RestaurantId == _currentRestaurantId);
// modelBuilder.Entity<MenuItem>().HasQueryFilter(m => m.RestaurantId == _currentRestaurantId);

// REMOVED - No middleware to set current restaurant from JWT claim
// app.Use(async (context, next) =>
// {
//     var restaurantId = context.User.FindFirst("RestaurantId")?.Value;
//     _tenantContext.SetRestaurantId(Guid.Parse(restaurantId));
//     await next();
// });
```

~~**Security** (NOT APPLICABLE):~~
- ~~RestaurantId in JWT claims (signed, tamper-proof)~~
- ~~All queries automatically scoped to current restaurant (FR-013)~~
- ~~Audit log includes RestaurantId for compliance~~

~~**Scale Limits**: 100 restaurants per instance (FR-118)~~ - NOT APPLICABLE

---

### Single-Restaurant Simplifications

**Decision**: Optimize architecture for single-restaurant operation

**Key Simplifications**:

1. **No Multi-Tenant Filtering**:
   - No RestaurantId column needed on most entities
   - No global query filters required
   - Simpler queries without tenant scoping overhead
   - Restaurant configuration is a singleton entity

2. **Simplified Authentication**:
   - User authentication → Role-based authorization only
   - No RestaurantId claim in JWT tokens
   - Direct user → roles mapping (Admin, Waiter, Chef, Guest)
   - Single restaurant context is implicit

3. **Restaurant Configuration as Singleton**:
   - Single RestaurantConfig entity (not per-tenant)
   - Configuration loaded once at startup
   - No need for restaurant selection or switching logic
   - Restaurant settings accessible globally without context

4. **Reduced Index Complexity**:
   - Indexes optimized for single dataset
   - No composite indexes with RestaurantId prefix
   - Simpler partition strategies (by date, not by restaurant+date)

5. **No Tenant Isolation Middleware**:
   - No tenant context service needed
   - No middleware to extract and validate RestaurantId
   - Reduced application startup complexity
   - Fewer points of failure in request pipeline

6. **Simplified Caching**:
   - Cache keys without restaurant prefix: `menu:items` instead of `menu:{restaurantId}:items`
   - Smaller cache footprint
   - No cross-tenant cache pollution concerns

7. **Deployment & Scaling**:
   - Single database schema (no per-tenant databases or schemas)
   - Horizontal scaling without tenant affinity concerns
   - Simpler backup and restore procedures
   - No tenant-specific configuration management

**Future Multi-Tenant Migration Path** (if needed):
If the system needs to scale to multiple restaurants in the future:
1. Add RestaurantId columns to relevant entities
2. Implement global query filters in EF Core
3. Add RestaurantId claim to JWT tokens
4. Implement tenant context middleware
5. Update indexes to include RestaurantId
6. Migrate existing data with default RestaurantId

---

### Caching Strategy

**Decision**: Redis for distributed caching and session state

**Rationale**:
- Menu data cache (high read, low write) - TTL 5 minutes
- SignalR backplane for scaling
- Distributed session state for load balancing
- Queue for background jobs (alternative: Hangfire, RabbitMQ)

**Cache Patterns**:
```csharp
// Cache-aside for menu data (single restaurant)
public async Task<List<MenuItem>> GetMenuAsync()
{
    var cacheKey = "menu:items"; // No restaurantId needed
    var cached = await _cache.GetStringAsync(cacheKey);
    if (cached != null) return JsonSerializer.Deserialize<List<MenuItem>>(cached);

    var menu = await _db.MenuItems.ToListAsync(); // No restaurant filtering needed
    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(menu),
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
    return menu;
}
```

**Invalidation**:
- On menu item create/update/delete → evict cache
- SignalR notification triggers client-side cache refresh

---

### Logging & Observability

**Decision**: Serilog + OpenTelemetry + Prometheus

**Rationale**:
- **Serilog**: Structured logging with JSON output for log aggregation (Seq, ELK, CloudWatch)
- **OpenTelemetry**: Distributed tracing across services (FR-127)
- **Prometheus**: Metrics endpoint for monitoring (FR-126)

**Configuration**:
```csharp
// Serilog with enrichment
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "RestaurantSuite.Api")
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

// OpenTelemetry
services.AddOpenTelemetry()
    .WithTracing(builder => builder.AddAspNetCoreInstrumentation().AddEntityFrameworkCoreInstrumentation())
    .WithMetrics(builder => builder.AddAspNetCoreInstrumentation().AddPrometheusExporter());
```

**Key Metrics**:
- Order placement latency (p50, p95, p99)
- SignalR connection count
- Database query duration
- API request rate and error rate

---

### Testing Strategy

**Decision**: xUnit + Testcontainers + Playwright

**Rationale**:
- **xUnit**: Standard for .NET, parallel test execution
- **Testcontainers**: Spin up real PostgreSQL/SQL Server for integration tests
- **Playwright**: E2E tests for Blazor UI (FR-testing)

**Test Pyramid**:
```
E2E (Playwright) - 5%
  └─ Critical user flows: order → kitchen → payment

Integration (xUnit + Testcontainers) - 25%
  └─ API controllers, EF repositories, SignalR hubs

Unit (xUnit) - 70%
  └─ Domain logic, validators, services
```

**TDD Workflow**:
1. Write contract tests (OpenAPI validation)
2. Write integration tests for user stories
3. Tests fail (red)
4. Implement minimal code to pass (green)
5. Refactor

**Example Integration Test**:
```csharp
public class OrderIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task PlaceOrder_NotifiesKitchen_ViaSignalR()
    {
        // Arrange: Testcontainer DB + SignalR client
        // Act: POST /api/orders
        // Assert: Kitchen client receives OrderCreated message
    }
}
```

---

### Docker & Deployment

**Decision**: Multi-stage Dockerfiles + docker-compose for local dev

**Rationale**:
- Multi-stage builds for small production images (~100MB .NET runtime)
- docker-compose orchestrates API + DB + Redis + frontends locally
- Kubernetes-ready (deployment manifests in `/infra/k8s/`)

**docker-compose.yml Structure**:
```yaml
services:
  api:
    build: ./src/RestaurantSuite.Api
    ports: ["5000:8080"]
    depends_on: [postgres, redis]

  postgres:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: dev

  redis:
    image: redis:7-alpine

  admin:
    build: ./src/RestaurantSuite.Admin
    ports: ["5001:8080"]
```

**CI/CD Pipeline** (GitHub Actions):
1. Build all projects
2. Run unit + integration tests (Testcontainers)
3. Build Docker images
4. Push to registry (Azure ACR, Docker Hub)
5. Run database migrations (EF Core Bundle)
6. Deploy to Azure App Service / Azure Container Apps

---

### Localization & Internationalization

**Decision**: .NET Resource files + database-stored menu translations

**Rationale**:
- UI strings: `.resx` files with `IStringLocalizer` (FR-109)
- Menu items: Translations table for user-generated content (FR-112)
- Currency formatting: `CultureInfo` based on restaurant config (FR-110)
- Timezone handling: NodaTime for accurate conversions (FR-111)

**Implementation**:
```csharp
// UI localization
services.AddLocalization(options => options.ResourcesPath = "Resources");
services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en-US", "es-ES", "fr-FR" };
    options.SetDefaultCulture("en-US")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

// Menu translations
public class MenuItemTranslation
{
    public Guid MenuItemId { get; set; }
    public string LanguageCode { get; set; } // "en", "es", "fr"
    public string Name { get; set; }
    public string Description { get; set; }
}
```

---

### Data Retention & GDPR Compliance

**Decision**: Soft delete + data export + scheduled purge jobs

**Rationale**:
- Soft delete for audit trail (IsDeleted flag)
- Data export API: `/api/users/{id}/export` → JSON (FR-114)
- Right to be forgotten: Anonymize PII after deletion request
- Scheduled job purges anonymous orders after 90 days (FR-113)

**Implementation**:
```csharp
// Soft delete global filter
modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

// Anonymization on deletion
public async Task DeleteUserDataAsync(Guid userId)
{
    var user = await _db.Users.FindAsync(userId);
    user.Email = $"deleted_{userId}@anonymous.local";
    user.Phone = null;
    user.IsDeleted = true;
    await _db.SaveChangesAsync();
}
```

**Retention Policies**:
- Authenticated users: Indefinite until deletion request (FR-113)
- Anonymous orders: 90 days (FR-113)
- Audit logs: 1 year operational, 5-7 years compliance (FR-097)

---

## Performance & Scalability

### Database Optimization

**Decisions**:
- Indexes on foreign keys and common query patterns (CreatedAt for reports, Status for filtering)
- Denormalized order totals (avoid recalculating on every query)
- Partitioning on AuditLog table by month (for large datasets)
- Read replicas for reporting queries (separate from transactional load)
- No RestaurantId indexes needed (single-restaurant architecture)

### API Performance

**Targets** (FR-117):
- <500ms p95 for critical operations (GET menu, POST order, PUT order status)
- Caching for read-heavy endpoints (menu, table status)
- Pagination on list endpoints (max 100 items per page)
- Compression middleware (Gzip/Brotli)

**Load Testing**:
- K6 or NBomber for API load tests
- Target: 500 concurrent users, 20 orders/second sustained

### SignalR Scalability

**Decisions**:
- Redis backplane for horizontal scaling
- Connection throttling: Max 10k connections per instance
- Group-based messaging (by role/station: kitchen, waiters, admin) for targeted notifications
- Automatic reconnection with exponential backoff
- No per-restaurant groups needed (single restaurant)

---

## Security Hardening

**Decisions**:
- HTTPS only (HSTS enabled)
- CORS policy: Whitelist specific origins
- Rate limiting: `AspNetCoreRateLimit` middleware
- SQL injection prevention: Parameterized queries (EF Core enforces)
- XSS prevention: Blazor auto-escapes output
- CSRF protection: Anti-forgery tokens on forms
- Secrets management: Azure Key Vault or AWS Secrets Manager
- Dependency scanning: Dependabot, Snyk

---

## Conclusion

All technical decisions are now documented with rationale and implementation guidance. No NEEDS CLARIFICATION items remain. The stack is production-ready with industry best practices for single-restaurant operation, real-time communication, security, and scalability.

**Architecture Decision**: The system has been simplified from multi-tenant to single-restaurant architecture for "Restoran Kuzma", eliminating tenant isolation complexity while maintaining all core functionality. A migration path to multi-tenancy is documented if future expansion is needed.

**Next Steps**: Proceed to Phase 1 - Design data model, API contracts, and quickstart guide.
