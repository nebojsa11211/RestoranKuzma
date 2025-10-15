# Database Browser API - Best Practices & Recommendations

## Table of Contents

1. [Security Best Practices](#security-best-practices)
2. [Performance Optimization](#performance-optimization)
3. [Error Handling](#error-handling)
4. [Testing Guidelines](#testing-guidelines)
5. [Production Deployment](#production-deployment)
6. [Monitoring & Logging](#monitoring--logging)
7. [Client Integration](#client-integration)
8. [Common Pitfalls](#common-pitfalls)

---

## Security Best Practices

### 1. Authentication & Authorization

**Always validate the user's role before granting access:**

```csharp
// Controller level (REQUIRED)
[Authorize(Roles = "Admin")]

// Additional validation in action methods (optional but recommended)
var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
if (userRole != "Admin")
{
    return Forbid();
}
```

**Best Practice**: Use policy-based authorization for more granular control:

```csharp
// In Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DatabaseBrowserAccess", policy =>
        policy.RequireRole("Admin")
              .RequireClaim("DatabaseAccess", "Full"));
});

// In Controller
[Authorize(Policy = "DatabaseBrowserAccess")]
```

### 2. Sensitive Data Protection

**Maintain an up-to-date list of sensitive columns:**

```csharp
// Regular review and update of sensitive columns
private static readonly HashSet<string> SensitiveColumns = new(StringComparer.OrdinalIgnoreCase)
{
    // Authentication & Security
    "PasswordHash",
    "Password",
    "RefreshToken",
    "Secret",
    "ApiKey",
    "Salt",

    // Personal Information (PII)
    "SSN",
    "TaxId",
    "CreditCardNumber",
    "BankAccountNumber",

    // Sensitive Business Data
    "EncryptionKey",
    "PrivateKey",
    "AccessToken",
    "SecurityAnswer"
};
```

**Best Practice**: Consider column-level encryption for highly sensitive data:

```csharp
// Use EF Core value converters for automatic encryption/decryption
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .Property(u => u.SSN)
        .HasConversion(
            v => EncryptionService.Encrypt(v),
            v => EncryptionService.Decrypt(v)
        );
}
```

### 3. SQL Injection Prevention

**Always use parameterized queries through EF Core:**

```csharp
// GOOD - Parameterized via EF Core
query = query.Where($"{propertyName} == @0", convertedValue);

// BAD - String interpolation (vulnerable to injection)
query = query.Where($"{propertyName} == '{value}'");
```

**Validate all input:**

```csharp
// Validate table names against whitelist
if (!_databaseBrowserService.IsValidTableName(tableName))
{
    return BadRequest("Invalid table name");
}

// Validate column names against entity metadata
var property = entityType.GetProperties()
    .FirstOrDefault(p => string.Equals(p.GetColumnName(), columnName, StringComparison.OrdinalIgnoreCase));

if (property == null)
{
    return BadRequest($"Column '{columnName}' does not exist");
}
```

### 4. Rate Limiting

**Implement rate limiting to prevent abuse:**

```csharp
// Install: AspNetCoreRateLimit
// In Program.cs
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "GET:/api/admin/database/*",
            Period = "1m",
            Limit = 30
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/admin/database/*",
            Period = "1m",
            Limit = 10
        }
    };
});
```

### 5. Audit Logging

**Log all database browser access:**

```csharp
_logger.LogInformation(
    "Admin {AdminId} ({AdminEmail}) accessed table {TableName} - Page: {Page}, Filters: {FilterCount}",
    userId,
    userEmail,
    tableName,
    request.Page,
    request.Filters?.Count ?? 0
);

// For compliance, consider structured logging to external system
_auditLogger.LogDatabaseAccess(new DatabaseAccessAudit
{
    UserId = userId,
    UserEmail = userEmail,
    TableName = tableName,
    Operation = "Query",
    Timestamp = DateTime.UtcNow,
    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
    Filters = request.Filters,
    RowsReturned = response.Rows.Count
});
```

---

## Performance Optimization

### 1. Caching Strategy

**Use appropriate cache durations:**

```csharp
// Static metadata - longer cache
_cache.Set(schemaKey, schema, new MemoryCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
    SlidingExpiration = TimeSpan.FromMinutes(5)
});

// Semi-static data - shorter cache
_cache.Set(tableListKey, tables, TimeSpan.FromMinutes(5));

// Dynamic data - no caching or very short cache
// (table data should not be cached as it changes frequently)
```

**Consider distributed caching for multi-server deployments:**

```csharp
// In Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
    options.InstanceName = "DatabaseBrowser:";
});
```

### 2. Pagination Best Practices

**Always use pagination for large datasets:**

```csharp
// Default page size should be reasonable
public const int DEFAULT_PAGE_SIZE = 50;
public const int MAX_PAGE_SIZE = 1000;

// Validate and enforce limits
request.PageSize = Math.Min(request.PageSize, MAX_PAGE_SIZE);
request.PageSize = Math.Max(request.PageSize, 1);
```

**Consider cursor-based pagination for very large tables:**

```csharp
// For tables with millions of rows, cursor-based is more efficient
public class CursorPagedRequest
{
    public string? Cursor { get; set; }  // Last Id from previous page
    public int PageSize { get; set; } = 50;
}

// Implementation
var query = dbSet.Where(e => e.Id > cursorId).OrderBy(e => e.Id).Take(pageSize);
```

### 3. Query Optimization

**Use AsNoTracking for read-only queries:**

```csharp
// Database browser is read-only, so disable change tracking
var dbSet = (dbSetProperty.GetValue(_context) as IQueryable<object>)?.AsNoTracking();
```

**Add database indexes for frequently queried columns:**

```csharp
// In ApplicationDbContext
modelBuilder.Entity<Order>()
    .HasIndex(e => new { e.Status, e.CreatedAt })
    .HasDatabaseName("IX_Orders_Status_CreatedAt");

modelBuilder.Entity<User>()
    .HasIndex(e => e.Email)
    .IsUnique();
```

**Use projection to select only needed columns:**

```csharp
// For very wide tables, consider allowing column selection
public class TableDataRequest
{
    public List<string>? SelectedColumns { get; set; }  // Optional column filtering
}

// In service
if (request.SelectedColumns != null && request.SelectedColumns.Any())
{
    // Project only selected columns
    properties = properties.Where(p => request.SelectedColumns.Contains(p.GetColumnName()));
}
```

### 4. Memory Management

**Limit result set sizes:**

```csharp
// Set hard limits to prevent out-of-memory issues
public const int ABSOLUTE_MAX_PAGE_SIZE = 10000;

if (request.PageSize > ABSOLUTE_MAX_PAGE_SIZE)
{
    throw new ArgumentException($"Page size cannot exceed {ABSOLUTE_MAX_PAGE_SIZE}");
}
```

**Use streaming for large exports:**

```csharp
// For CSV/Excel exports, use streaming instead of loading all data
public async Task<FileStreamResult> ExportTableDataAsync(string tableName)
{
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);

    // Write header
    await writer.WriteLineAsync(string.Join(",", columns));

    // Stream data in chunks
    var pageSize = 1000;
    var page = 1;
    TableDataResponse data;

    do
    {
        data = await GetTableDataAsync(tableName, new TableDataRequest { Page = page, PageSize = pageSize });

        foreach (var row in data.Rows)
        {
            await writer.WriteLineAsync(string.Join(",", row.Values));
        }

        page++;
    } while (data.HasNextPage);

    await writer.FlushAsync();
    stream.Position = 0;

    return new FileStreamResult(stream, "text/csv")
    {
        FileDownloadName = $"{tableName}_{DateTime.UtcNow:yyyyMMdd}.csv"
    };
}
```

---

## Error Handling

### 1. Comprehensive Exception Handling

**Use specific exception types:**

```csharp
try
{
    return await _databaseBrowserService.GetTableDataAsync(tableName, request);
}
catch (InvalidTableNameException ex)
{
    _logger.LogWarning(ex, "Invalid table name requested: {TableName}", tableName);
    return BadRequest(new { success = false, message = ex.Message });
}
catch (UnauthorizedAccessException ex)
{
    _logger.LogWarning(ex, "Unauthorized access attempt");
    return Forbid();
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Database error");
    return StatusCode(500, new { success = false, message = "Database error occurred" });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
    return StatusCode(500, new { success = false, message = "An unexpected error occurred" });
}
```

### 2. Validation Error Messages

**Provide clear, actionable error messages:**

```csharp
// GOOD
return BadRequest(new
{
    success = false,
    message = "Page size must be between 1 and 1000",
    field = "pageSize",
    value = request.PageSize,
    validRange = new { min = 1, max = 1000 }
});

// BAD
return BadRequest("Invalid input");
```

### 3. Graceful Degradation

**Handle missing data gracefully:**

```csharp
// If row count query fails, return estimate or 0
private async Task<long> GetEstimatedRowCountAsync(string tableName)
{
    try
    {
        return await GetExactRowCountAsync(tableName);
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Could not get row count for {TableName}, returning estimate", tableName);
        return 0; // or return cached value
    }
}
```

---

## Testing Guidelines

### 1. Unit Tests

**Test service layer thoroughly:**

```csharp
[Fact]
public async Task GetTablesAsync_ExcludesBlacklistedTables()
{
    // Arrange
    var context = GetInMemoryContext();
    var cache = new MemoryCache(new MemoryCacheOptions());
    var logger = new Mock<ILogger<DatabaseBrowserService>>();
    var service = new DatabaseBrowserService(context, cache, logger.Object);

    // Act
    var tables = await service.GetTablesAsync();

    // Assert
    Assert.DoesNotContain(tables, t => t.TableName == "__EFMigrationsHistory");
}

[Fact]
public async Task GetTableDataAsync_RedactsSensitiveColumns()
{
    // Arrange
    var service = GetService();
    var request = new TableDataRequest { Page = 1, PageSize = 10 };

    // Act
    var result = await service.GetTableDataAsync("Users", request);

    // Assert
    var passwordHashValues = result.Rows.Select(r => r["PasswordHash"]);
    Assert.All(passwordHashValues, v => Assert.Equal("[REDACTED]", v));
}
```

### 2. Integration Tests

**Test controller endpoints with authentication:**

```csharp
[Fact]
public async Task GetTables_RequiresAdminRole()
{
    // Arrange
    var client = _factory.CreateClient();
    var token = await GetUserToken("user@test.com", "User"); // Non-admin

    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

    // Act
    var response = await client.GetAsync("/api/admin/database/tables");

    // Assert
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

### 3. Performance Tests

**Benchmark critical operations:**

```csharp
[Fact]
public async Task GetTableData_CompletesWithin500ms_ForSmallTables()
{
    // Arrange
    var service = GetService();
    var request = new TableDataRequest { Page = 1, PageSize = 50 };
    var stopwatch = Stopwatch.StartNew();

    // Act
    var result = await service.GetTableDataAsync("Users", request);

    // Assert
    stopwatch.Stop();
    Assert.True(stopwatch.ElapsedMilliseconds < 500,
        $"Query took {stopwatch.ElapsedMilliseconds}ms, expected < 500ms");
}
```

---

## Production Deployment

### 1. Configuration

**Use environment-specific settings:**

```json
// appsettings.Production.json
{
  "DatabaseBrowser": {
    "EnableInProduction": false,  // Disable by default in production
    "MaxPageSize": 500,           // Lower limit in production
    "CacheDurationMinutes": 15,   // Longer cache in production
    "RequireMfa": true,           // Require MFA for database access
    "AllowedIpAddresses": [       // IP whitelist for additional security
      "10.0.0.0/8",
      "192.168.1.0/24"
    ]
  }
}
```

**Implement feature toggle:**

```csharp
// In Program.cs
var enableDatabaseBrowser = builder.Configuration.GetValue<bool>("DatabaseBrowser:EnableInProduction");

if (builder.Environment.IsDevelopment() || enableDatabaseBrowser)
{
    builder.Services.AddScoped<IDatabaseBrowserService, DatabaseBrowserService>();
}
else
{
    builder.Services.AddScoped<IDatabaseBrowserService, DisabledDatabaseBrowserService>();
}
```

### 2. Health Checks

**Add health check endpoint:**

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddCheck("DatabaseBrowserCache", () =>
    {
        // Check if cache is working
        var cache = serviceProvider.GetService<IMemoryCache>();
        return cache != null ? HealthCheckResult.Healthy() : HealthCheckResult.Degraded();
    });

app.MapHealthChecks("/health");
```

### 3. SSL/TLS

**Always use HTTPS in production:**

```csharp
// In Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}
```

---

## Monitoring & Logging

### 1. Structured Logging

**Use structured logging with correlation IDs:**

```csharp
using var scope = _logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = userId,
    ["TableName"] = tableName,
    ["CorrelationId"] = HttpContext.TraceIdentifier
});

_logger.LogInformation(
    "Database browser query - Table: {TableName}, Page: {Page}, Filters: {FilterCount}",
    tableName,
    request.Page,
    request.Filters?.Count ?? 0
);
```

### 2. Performance Metrics

**Track key metrics:**

```csharp
// Install: App.Metrics
var metrics = new MetricsBuilder()
    .Report.ToConsole()
    .Build();

// Track query duration
using (metrics.Measure.Timer.Time(DatabaseBrowserMetrics.QueryDuration, new MetricTags("table", tableName)))
{
    result = await _databaseBrowserService.GetTableDataAsync(tableName, request);
}

// Track cache hit rate
metrics.Measure.Counter.Increment(DatabaseBrowserMetrics.CacheHits);
```

### 3. Alerting

**Set up alerts for suspicious activity:**

```csharp
// Alert on excessive failed authorization attempts
if (failedAttempts > 10)
{
    await _alertService.SendAlert(new Alert
    {
        Severity = AlertSeverity.High,
        Message = $"Multiple unauthorized database browser access attempts from {ipAddress}",
        Timestamp = DateTime.UtcNow
    });
}

// Alert on large data exports
if (request.PageSize > 5000)
{
    await _alertService.SendAlert(new Alert
    {
        Severity = AlertSeverity.Medium,
        Message = $"Large data export requested by {userEmail} - Table: {tableName}, Size: {request.PageSize}",
        Timestamp = DateTime.UtcNow
    });
}
```

---

## Client Integration

### 1. TypeScript Client

**Create type-safe client:**

```typescript
// database-browser-client.ts
export class DatabaseBrowserClient {
  constructor(
    private baseUrl: string,
    private getAccessToken: () => Promise<string>
  ) {}

  async getTables(): Promise<DatabaseTableInfo[]> {
    const response = await fetch(`${this.baseUrl}/api/admin/database/tables`, {
      headers: await this.getAuthHeaders()
    });

    if (!response.ok) {
      throw new DatabaseBrowserError('Failed to fetch tables', response.status);
    }

    return await response.json();
  }

  async getTableSchema(tableName: string): Promise<TableSchemaResponse> {
    const response = await fetch(
      `${this.baseUrl}/api/admin/database/tables/${encodeURIComponent(tableName)}/schema`,
      { headers: await this.getAuthHeaders() }
    );

    if (!response.ok) {
      throw new DatabaseBrowserError(`Failed to fetch schema for ${tableName}`, response.status);
    }

    return await response.json();
  }

  private async getAuthHeaders(): Promise<HeadersInit> {
    const token = await this.getAccessToken();
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }
}
```

### 2. React Component Example

**Build reusable components:**

```typescript
// DatabaseBrowser.tsx
const DatabaseBrowser: React.FC = () => {
  const [tables, setTables] = useState<DatabaseTableInfo[]>([]);
  const [selectedTable, setSelectedTable] = useState<string | null>(null);
  const [tableData, setTableData] = useState<TableDataResponse | null>(null);

  useEffect(() => {
    loadTables();
  }, []);

  const loadTables = async () => {
    const client = new DatabaseBrowserClient(API_BASE_URL, getAccessToken);
    const tables = await client.getTables();
    setTables(tables);
  };

  const loadTableData = async (tableName: string, page: number = 1) => {
    const client = new DatabaseBrowserClient(API_BASE_URL, getAccessToken);
    const data = await client.getTableData(tableName, { page, pageSize: 50 });
    setTableData(data);
  };

  return (
    <div className="database-browser">
      <TableList tables={tables} onSelectTable={setSelectedTable} />
      {selectedTable && (
        <TableDataView
          tableName={selectedTable}
          data={tableData}
          onPageChange={(page) => loadTableData(selectedTable, page)}
        />
      )}
    </div>
  );
};
```

---

## Common Pitfalls

### 1. Not Validating User Input

**Pitfall**: Trusting table/column names from user input

**Solution**: Always validate against entity metadata
```csharp
if (!_databaseBrowserService.IsValidTableName(tableName))
{
    return BadRequest("Invalid table name");
}
```

### 2. Exposing Too Much Data

**Pitfall**: Returning entire tables without pagination

**Solution**: Always enforce pagination
```csharp
// Enforce maximum page size
const int MAX_PAGE_SIZE = 1000;
request.PageSize = Math.Min(request.PageSize, MAX_PAGE_SIZE);
```

### 3. Not Handling Nullable Types

**Pitfall**: Crashes when converting null values

**Solution**: Handle nulls gracefully
```csharp
private object? ConvertToJsonFriendlyValue(object? value)
{
    if (value == null)
        return null;  // Explicit null check

    // ... rest of conversion logic
}
```

### 4. Forgetting to Redact Sensitive Data

**Pitfall**: Accidentally exposing passwords or tokens

**Solution**: Maintain comprehensive sensitive column list
```csharp
// Regular review of this list
private static readonly HashSet<string> SensitiveColumns = new(...)
```

### 5. Not Caching Metadata

**Pitfall**: Re-querying entity metadata on every request

**Solution**: Cache schema information
```csharp
if (_cache.TryGetValue(cacheKey, out TableSchemaResponse? cached))
{
    return cached;
}
```

### 6. Poor Error Messages

**Pitfall**: Generic "An error occurred" messages

**Solution**: Provide specific, actionable error messages
```csharp
return BadRequest(new {
    success = false,
    message = $"Column '{columnName}' does not exist in table '{tableName}'",
    availableColumns = schema.Columns.Select(c => c.ColumnName)
});
```

---

## Summary

Following these best practices will ensure your Database Browser API is:

- **Secure**: Multi-layered security with proper authentication, authorization, and data protection
- **Performant**: Efficient caching, pagination, and query optimization
- **Reliable**: Comprehensive error handling and graceful degradation
- **Maintainable**: Well-tested, documented, and monitored
- **Production-Ready**: Proper configuration, deployment, and monitoring strategies

Remember to regularly review and update your security settings, especially the lists of blacklisted tables and sensitive columns, as your application evolves.
