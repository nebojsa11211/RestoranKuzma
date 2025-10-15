# Database Browser Feature - Step-by-Step Implementation Guide

## Overview
This guide provides detailed, actionable steps to implement a complete database browser feature for the RestaurantSuite.Admin application. Follow these steps in order for best results.

**Estimated Total Time:** 22-30 hours
**Difficulty:** Intermediate to Advanced
**Prerequisites:**
- .NET 9 SDK installed
- Understanding of ASP.NET Core, EF Core, and Blazor
- Access to the RestaurantSuite codebase

---

## Phase 1: Backend Foundation (4-6 hours)

### Step 1: Create DTO Directory Structure (5 minutes)

**Action:** Create a new folder for database browser DTOs.

**Location:** `src/RestaurantSuite.Application/DTOs/DatabaseBrowser/`

**Commands:**
```powershell
cd D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Application\DTOs
mkdir DatabaseBrowser
cd DatabaseBrowser
```

**Files to Create:**
1. `TableMetadataDto.cs`
2. `TableSchemaDto.cs`
3. `ColumnMetadataDto.cs`
4. `TableDataRequest.cs`
5. `TableDataResultDto.cs`
6. `DatabaseStatisticsDto.cs`

---

### Step 2: Create TableMetadataDto.cs (10 minutes)

**File:** `src/RestaurantSuite.Application/DTOs/DatabaseBrowser/TableMetadataDto.cs`

**Code:**
```csharp
namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Represents basic metadata about a database table
/// </summary>
public class TableMetadataDto
{
    /// <summary>
    /// Physical table name in database (e.g., "Users")
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Display-friendly name (e.g., "User Accounts")
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Estimated row count (may be approximate for large tables)
    /// </summary>
    public int EstimatedRowCount { get; set; }

    /// <summary>
    /// Number of columns in the table
    /// </summary>
    public int ColumnCount { get; set; }

    /// <summary>
    /// Whether the table is accessible via this API
    /// </summary>
    public bool IsAccessible { get; set; } = true;

    /// <summary>
    /// Whether table contains sensitive data (PII, passwords, etc.)
    /// </summary>
    public bool HasSensitiveData { get; set; }

    /// <summary>
    /// CLR entity type name
    /// </summary>
    public string? EntityTypeName { get; set; }
}
```

**Verification:**
- Ensure namespace matches your project structure
- Build the Application project: `dotnet build`

---

### Step 3: Create ColumnMetadataDto.cs (10 minutes)

**File:** `src/RestaurantSuite.Application/DTOs/DatabaseBrowser/ColumnMetadataDto.cs`

**Code:**
```csharp
namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Represents metadata about a single column in a table
/// </summary>
public class ColumnMetadataDto
{
    /// <summary>
    /// Physical column name in database
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// CLR property name (may differ from column name)
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Data type (e.g., "String", "Int32", "DateTime", "Guid")
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// SQL database type (e.g., "varchar(100)", "int", "datetime2")
    /// </summary>
    public string? SqlDataType { get; set; }

    /// <summary>
    /// Whether column allows NULL values
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Whether this is a primary key column
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Whether this is a foreign key column
    /// </summary>
    public bool IsForeignKey { get; set; }

    /// <summary>
    /// Maximum length for string columns
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Whether this column contains sensitive data
    /// </summary>
    public bool IsSensitive { get; set; }

    /// <summary>
    /// Column ordinal position
    /// </summary>
    public int OrdinalPosition { get; set; }

    /// <summary>
    /// If foreign key, the related table name
    /// </summary>
    public string? ForeignKeyTable { get; set; }
}
```

---

### Step 4: Create TableSchemaDto.cs (10 minutes)

**File:** `src/RestaurantSuite.Application/DTOs/DatabaseBrowser/TableSchemaDto.cs`

**Code:**
```csharp
namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Represents complete schema information for a table
/// </summary>
public class TableSchemaDto
{
    /// <summary>
    /// Table name
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Entity type name
    /// </summary>
    public string EntityTypeName { get; set; } = string.Empty;

    /// <summary>
    /// List of all columns with their metadata
    /// </summary>
    public List<ColumnMetadataDto> Columns { get; set; } = new();

    /// <summary>
    /// Primary key column names
    /// </summary>
    public List<string> PrimaryKeyColumns { get; set; } = new();

    /// <summary>
    /// Foreign key relationships
    /// </summary>
    public List<ForeignKeyRelationship> ForeignKeys { get; set; } = new();

    /// <summary>
    /// Index information
    /// </summary>
    public List<IndexInfo> Indexes { get; set; } = new();
}

/// <summary>
/// Represents a foreign key relationship
/// </summary>
public class ForeignKeyRelationship
{
    public string ColumnName { get; set; } = string.Empty;
    public string ReferencedTable { get; set; } = string.Empty;
    public string ReferencedColumn { get; set; } = string.Empty;
    public string ConstraintName { get; set; } = string.Empty;
}

/// <summary>
/// Represents an index
/// </summary>
public class IndexInfo
{
    public string IndexName { get; set; } = string.Empty;
    public List<string> Columns { get; set; } = new();
    public bool IsUnique { get; set; }
}
```

---

### Step 5: Create TableDataRequest.cs (10 minutes)

**File:** `src/RestaurantSuite.Application/DTOs/DatabaseBrowser/TableDataRequest.cs`

**Code:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Request for querying table data with filtering, sorting, and pagination
/// </summary>
public class TableDataRequest
{
    /// <summary>
    /// Table name to query
    /// </summary>
    [Required]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of records per page (max 1000)
    /// </summary>
    [Range(1, 1000)]
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// Column name to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction: "asc" or "desc"
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Global search term (searches across all string columns)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Advanced filters
    /// </summary>
    public List<ColumnFilter>? Filters { get; set; }
}

/// <summary>
/// Represents a filter on a specific column
/// </summary>
public class ColumnFilter
{
    /// <summary>
    /// Column name to filter
    /// </summary>
    [Required]
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Filter operator: equals, notEquals, contains, startsWith, endsWith,
    /// greaterThan, lessThan, greaterThanOrEqual, lessThanOrEqual, isNull, isNotNull
    /// </summary>
    [Required]
    public string Operator { get; set; } = "equals";

    /// <summary>
    /// Value to compare against (null for isNull/isNotNull operators)
    /// </summary>
    public object? Value { get; set; }
}
```

---

### Step 6: Create TableDataResultDto.cs (10 minutes)

**File:** `src/RestaurantSuite.Application/DTOs/DatabaseBrowser/TableDataResultDto.cs`

**Code:**
```csharp
namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Result of a table data query with pagination information
/// </summary>
public class TableDataResultDto
{
    /// <summary>
    /// Table name that was queried
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// List of data rows (each row is a dictionary of column name -> value)
    /// </summary>
    public List<Dictionary<string, object?>> Data { get; set; } = new();

    /// <summary>
    /// Total number of records matching the query (before pagination)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Records per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
```

---

### Step 7: Create DatabaseStatisticsDto.cs (5 minutes)

**File:** `src/RestaurantSuite.Application/DTOs/DatabaseBrowser/DatabaseStatisticsDto.cs`

**Code:**
```csharp
namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Overall database statistics
/// </summary>
public class DatabaseStatisticsDto
{
    /// <summary>
    /// Total number of tables
    /// </summary>
    public int TotalTables { get; set; }

    /// <summary>
    /// Total number of rows across all tables
    /// </summary>
    public long TotalRows { get; set; }

    /// <summary>
    /// Name of the largest table by row count
    /// </summary>
    public string LargestTable { get; set; } = string.Empty;

    /// <summary>
    /// Row count of the largest table
    /// </summary>
    public int LargestTableRowCount { get; set; }

    /// <summary>
    /// Database size in MB (if available)
    /// </summary>
    public double? DatabaseSizeMB { get; set; }
}
```

**Verification Step:**
```powershell
cd D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Application
dotnet build
```

Expected: Build succeeds with 0 errors.

---

### Step 8: Create Service Interface (15 minutes)

**File:** `src/RestaurantSuite.Application/Interfaces/IDatabaseBrowserService.cs`

**Code:**
```csharp
using RestaurantSuite.Application.DTOs.DatabaseBrowser;

namespace RestaurantSuite.Application.Interfaces;

/// <summary>
/// Service for browsing database tables and data
/// </summary>
public interface IDatabaseBrowserService
{
    /// <summary>
    /// Get metadata for all accessible tables
    /// </summary>
    Task<List<TableMetadataDto>> GetTablesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get detailed schema information for a specific table
    /// </summary>
    /// <param name="tableName">Physical table name</param>
    Task<TableSchemaDto?> GetTableSchemaAsync(string tableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated data from a table with optional filtering and sorting
    /// </summary>
    Task<TableDataResultDto> GetTableDataAsync(TableDataRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get overall database statistics
    /// </summary>
    Task<DatabaseStatisticsDto> GetDatabaseStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a table is accessible
    /// </summary>
    Task<bool> IsTableAccessibleAsync(string tableName, CancellationToken cancellationToken = default);
}
```

**Verification:**
```powershell
dotnet build
```

---

### Step 9: Add Required NuGet Packages (10 minutes)

**Action:** Add System.Linq.Dynamic.Core for dynamic LINQ queries.

**Location:** `src/RestaurantSuite.Infrastructure.EF/RestaurantSuite.Infrastructure.EF.csproj`

**Commands:**
```powershell
cd D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Infrastructure.EF
dotnet add package System.Linq.Dynamic.Core --version 1.4.5
```

**Verify the package was added:**
Open `RestaurantSuite.Infrastructure.EF.csproj` and confirm you see:
```xml
<PackageReference Include="System.Linq.Dynamic.Core" Version="1.4.5" />
```

---

### Step 10: Create DatabaseBrowserService Implementation (90-120 minutes)

**File:** `src/RestaurantSuite.Infrastructure.EF/Services/DatabaseBrowserService.cs`

This is a large file. I'll provide it in sections:

**Part 1: Class Setup and Constructor**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using RestaurantSuite.Application.DTOs.DatabaseBrowser;
using RestaurantSuite.Application.Interfaces;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace RestaurantSuite.Infrastructure.EF.Services;

/// <summary>
/// Service for browsing database tables and data using EF Core metadata
/// </summary>
public class DatabaseBrowserService : IDatabaseBrowserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseBrowserService> _logger;

    // Security: Tables to exclude from browsing
    private static readonly HashSet<string> BlacklistedTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "__EFMigrationsHistory"
    };

    // Security: Columns that should be masked/redacted
    private static readonly HashSet<string> SensitiveColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "PasswordHash",
        "Password",
        "RefreshToken",
        "SecurityStamp",
        "Secret",
        "ApiKey",
        "Salt",
        "Token"
    };

    public DatabaseBrowserService(
        ApplicationDbContext context,
        ILogger<DatabaseBrowserService> logger)
    {
        _context = context;
        _logger = logger;
    }
```

**Part 2: GetTablesAsync Implementation**

```csharp
    public async Task<List<TableMetadataDto>> GetTablesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching database table metadata");

        var tables = new List<TableMetadataDto>();

        foreach (var entityType in _context.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();

            if (string.IsNullOrEmpty(tableName) || BlacklistedTables.Contains(tableName))
            {
                continue;
            }

            var rowCount = await GetTableRowCountAsync(tableName, cancellationToken);
            var columns = entityType.GetProperties().ToList();
            var hasSensitiveData = columns.Any(p =>
                SensitiveColumns.Contains(p.GetColumnName() ?? p.Name));

            tables.Add(new TableMetadataDto
            {
                TableName = tableName,
                DisplayName = tableName,
                EstimatedRowCount = rowCount,
                ColumnCount = columns.Count,
                IsAccessible = true,
                HasSensitiveData = hasSensitiveData,
                EntityTypeName = entityType.ClrType.Name
            });
        }

        _logger.LogInformation("Found {Count} accessible tables", tables.Count);
        return tables.OrderBy(t => t.TableName).ToList();
    }
```

**Part 3: GetTableSchemaAsync Implementation**

```csharp
    public async Task<TableSchemaDto?> GetTableSchemaAsync(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching schema for table: {TableName}", tableName);

        var entityType = FindEntityType(tableName);
        if (entityType == null)
        {
            _logger.LogWarning("Table not found: {TableName}", tableName);
            return null;
        }

        var columns = new List<ColumnMetadataDto>();
        var ordinal = 0;

        foreach (var property in entityType.GetProperties().OrderBy(p => p.GetColumnOrder() ?? 999))
        {
            var columnName = property.GetColumnName() ?? property.Name;
            var foreignKeys = property.GetContainingForeignKeys().ToList();

            columns.Add(new ColumnMetadataDto
            {
                ColumnName = columnName,
                PropertyName = property.Name,
                DataType = property.ClrType.Name,
                SqlDataType = property.GetColumnType(),
                IsNullable = property.IsNullable,
                IsPrimaryKey = property.IsPrimaryKey(),
                IsForeignKey = foreignKeys.Any(),
                MaxLength = property.GetMaxLength(),
                IsSensitive = SensitiveColumns.Contains(columnName),
                OrdinalPosition = ordinal++,
                ForeignKeyTable = foreignKeys.FirstOrDefault()?.PrincipalEntityType.GetTableName()
            });
        }

        var primaryKeys = entityType.FindPrimaryKey()?.Properties
            .Select(p => p.GetColumnName() ?? p.Name)
            .ToList() ?? new List<string>();

        var foreignKeyRelationships = entityType.GetForeignKeys()
            .Select(fk => new ForeignKeyRelationship
            {
                ColumnName = fk.Properties.First().GetColumnName() ?? fk.Properties.First().Name,
                ReferencedTable = fk.PrincipalEntityType.GetTableName() ?? "",
                ReferencedColumn = fk.PrincipalKey.Properties.First().GetColumnName() ?? fk.PrincipalKey.Properties.First().Name,
                ConstraintName = fk.GetConstraintName() ?? ""
            })
            .ToList();

        var indexes = entityType.GetIndexes()
            .Select(idx => new IndexInfo
            {
                IndexName = idx.GetDatabaseName() ?? "",
                Columns = idx.Properties.Select(p => p.GetColumnName() ?? p.Name).ToList(),
                IsUnique = idx.IsUnique
            })
            .ToList();

        return new TableSchemaDto
        {
            TableName = tableName,
            EntityTypeName = entityType.ClrType.Name,
            Columns = columns,
            PrimaryKeyColumns = primaryKeys,
            ForeignKeys = foreignKeyRelationships,
            Indexes = indexes
        };
    }
```

**Part 4: GetTableDataAsync Implementation**

```csharp
    public async Task<TableDataResultDto> GetTableDataAsync(
        TableDataRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Fetching data for table: {TableName}, Page: {Page}, PageSize: {PageSize}",
            request.TableName, request.Page, request.PageSize);

        var entityType = FindEntityType(request.TableName);
        if (entityType == null)
        {
            throw new ArgumentException($"Table '{request.TableName}' not found or not accessible");
        }

        // Get the DbSet for this entity type using reflection
        var dbSetProperty = _context.GetType()
            .GetProperties()
            .FirstOrDefault(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                p.PropertyType.GetGenericArguments()[0] == entityType.ClrType);

        if (dbSetProperty == null)
        {
            throw new InvalidOperationException($"DbSet for table '{request.TableName}' not found");
        }

        var dbSet = dbSetProperty.GetValue(_context) as IQueryable;
        if (dbSet == null)
        {
            throw new InvalidOperationException($"Failed to get queryable for table '{request.TableName}'");
        }

        // Apply AsNoTracking for read-only performance
        dbSet = dbSet.AsNoTracking();

        // Apply global search if provided
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            dbSet = ApplyGlobalSearch(dbSet, entityType, request.SearchTerm);
        }

        // Apply column filters if provided
        if (request.Filters != null && request.Filters.Any())
        {
            dbSet = ApplyFilters(dbSet, entityType, request.Filters);
        }

        // Get total count before pagination
        var totalCount = await dbSet.CountAsync(cancellationToken);

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var direction = request.SortDirection?.ToLower() == "desc" ? "descending" : "ascending";
            dbSet = dbSet.OrderBy($"{request.SortBy} {direction}");
        }
        else
        {
            // Default sort by primary key if no sort specified
            var pkProperty = entityType.FindPrimaryKey()?.Properties.FirstOrDefault();
            if (pkProperty != null)
            {
                dbSet = dbSet.OrderBy($"{pkProperty.Name} ascending");
            }
        }

        // Apply pagination
        var skip = (request.Page - 1) * request.PageSize;
        dbSet = dbSet.Skip(skip).Take(request.PageSize);

        // Execute query and convert to dictionaries
        var entities = await MaterializeQueryAsync(dbSet, cancellationToken);
        var data = ConvertEntitiesToDictionaries(entities, entityType);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new TableDataResultDto
        {
            TableName = request.TableName,
            Data = data,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }
```

**Part 5: Helper Methods**

```csharp
    public async Task<DatabaseStatisticsDto> GetDatabaseStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var tables = await GetTablesAsync(cancellationToken);

        return new DatabaseStatisticsDto
        {
            TotalTables = tables.Count,
            TotalRows = tables.Sum(t => (long)t.EstimatedRowCount),
            LargestTable = tables.OrderByDescending(t => t.EstimatedRowCount).FirstOrDefault()?.TableName ?? "N/A",
            LargestTableRowCount = tables.Max(t => t.EstimatedRowCount)
        };
    }

    public Task<bool> IsTableAccessibleAsync(string tableName, CancellationToken cancellationToken = default)
    {
        if (BlacklistedTables.Contains(tableName))
        {
            return Task.FromResult(false);
        }

        var entityType = FindEntityType(tableName);
        return Task.FromResult(entityType != null);
    }

    // Private helper methods

    private IEntityType? FindEntityType(string tableName)
    {
        return _context.Model.GetEntityTypes()
            .FirstOrDefault(e =>
                e.GetTableName()?.Equals(tableName, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task<int> GetTableRowCountAsync(string tableName, CancellationToken cancellationToken)
    {
        try
        {
            // Use raw SQL for efficient counting
            var connection = _context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM {tableName}";

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get row count for table {TableName}", tableName);
            return 0;
        }
    }

    private IQueryable ApplyGlobalSearch(IQueryable query, IEntityType entityType, string searchTerm)
    {
        // Build OR expression for all string properties
        var stringProperties = entityType.GetProperties()
            .Where(p => p.ClrType == typeof(string) &&
                       !SensitiveColumns.Contains(p.GetColumnName() ?? p.Name))
            .ToList();

        if (!stringProperties.Any())
        {
            return query;
        }

        var conditions = stringProperties
            .Select(p => $"{p.Name} != null && {p.Name}.Contains(@0)")
            .ToList();

        var whereClause = string.Join(" || ", conditions);

        return query.Where(whereClause, searchTerm);
    }

    private IQueryable ApplyFilters(IQueryable query, IEntityType entityType, List<ColumnFilter> filters)
    {
        foreach (var filter in filters)
        {
            var property = entityType.GetProperties()
                .FirstOrDefault(p => p.Name.Equals(filter.ColumnName, StringComparison.OrdinalIgnoreCase));

            if (property == null)
            {
                _logger.LogWarning("Filter column not found: {ColumnName}", filter.ColumnName);
                continue;
            }

            query = filter.Operator.ToLower() switch
            {
                "equals" => query.Where($"{property.Name} == @0", filter.Value),
                "notequals" => query.Where($"{property.Name} != @0", filter.Value),
                "contains" => query.Where($"{property.Name}.Contains(@0)", filter.Value),
                "startswith" => query.Where($"{property.Name}.StartsWith(@0)", filter.Value),
                "endswith" => query.Where($"{property.Name}.EndsWith(@0)", filter.Value),
                "greaterthan" => query.Where($"{property.Name} > @0", filter.Value),
                "lessthan" => query.Where($"{property.Name} < @0", filter.Value),
                "greaterthanorequal" => query.Where($"{property.Name} >= @0", filter.Value),
                "lessthanorequal" => query.Where($"{property.Name} <= @0", filter.Value),
                "isnull" => query.Where($"{property.Name} == null"),
                "isnotnull" => query.Where($"{property.Name} != null"),
                _ => query
            };
        }

        return query;
    }

    private async Task<List<object>> MaterializeQueryAsync(IQueryable query, CancellationToken cancellationToken)
    {
        // Use Dynamic LINQ to convert IQueryable to List
        var toListMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods()
            .First(m => m.Name == "ToListAsync" && m.GetParameters().Length == 2)
            .MakeGenericMethod(query.ElementType);

        var task = (Task)toListMethod.Invoke(null, new object[] { query, cancellationToken })!;
        await task;

        var resultProperty = task.GetType().GetProperty("Result");
        var result = resultProperty!.GetValue(task) as System.Collections.IEnumerable;

        return result?.Cast<object>().ToList() ?? new List<object>();
    }

    private List<Dictionary<string, object?>> ConvertEntitiesToDictionaries(
        List<object> entities,
        IEntityType entityType)
    {
        var result = new List<Dictionary<string, object?>>();

        foreach (var entity in entities)
        {
            var row = new Dictionary<string, object?>();

            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName() ?? property.Name;
                var propertyInfo = entityType.ClrType.GetProperty(property.Name);

                if (propertyInfo == null) continue;

                var value = propertyInfo.GetValue(entity);

                // Mask sensitive data
                if (SensitiveColumns.Contains(columnName))
                {
                    row[columnName] = "[REDACTED]";
                }
                else
                {
                    row[columnName] = FormatValue(value);
                }
            }

            result.Add(row);
        }

        return result;
    }

    private object? FormatValue(object? value)
    {
        if (value == null) return null;

        var type = value.GetType();

        // Handle common types
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
            type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
            type == typeof(Guid) || type == typeof(TimeSpan) || type.IsEnum)
        {
            return value;
        }

        // For complex types, return type name
        return $"[{type.Name}]";
    }
}
```

**Verification:**
```powershell
cd D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Infrastructure.EF
dotnet build
```

Expected: Build succeeds.

---

### Step 11: Register Service in DI Container (15 minutes)

**File:** `src/RestaurantSuite.Api/Program.cs`

**Action:** Add service registration and memory caching.

**Find this section:**
```csharp
// Add services to the container.
```

**Add after it:**
```csharp
// Memory cache for database browser
builder.Services.AddMemoryCache();

// Database browser service
builder.Services.AddScoped<IDatabaseBrowserService, DatabaseBrowserService>();
```

**Full context:**
```csharp
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Infrastructure.EF.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Memory cache for database browser
builder.Services.AddMemoryCache();

// Database browser service
builder.Services.AddScoped<IDatabaseBrowserService, DatabaseBrowserService>();

// ... rest of your Program.cs
```

**Verification:**
```powershell
cd D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Api
dotnet build
```

---

### Step 12: Create DatabaseBrowserController (45 minutes)

**File:** `src/RestaurantSuite.Api/Controllers/DatabaseBrowserController.cs`

**Complete Code:**
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RestaurantSuite.Application.DTOs.DatabaseBrowser;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Api.Controllers;

/// <summary>
/// Admin-only controller for browsing database tables and data
/// </summary>
[ApiController]
[Route("api/admin/database")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class DatabaseBrowserController : ControllerBase
{
    private readonly IDatabaseBrowserService _browserService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DatabaseBrowserController> _logger;

    public DatabaseBrowserController(
        IDatabaseBrowserService browserService,
        IMemoryCache cache,
        ILogger<DatabaseBrowserController> logger)
    {
        _browserService = browserService;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Get list of all accessible database tables with metadata
    /// </summary>
    /// <returns>List of table metadata</returns>
    [HttpGet("tables")]
    [ProducesResponseType(typeof(List<TableMetadataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<TableMetadataDto>>> GetTables(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {User} requesting table list", User.Identity?.Name);

        // Cache table list for 5 minutes (metadata rarely changes)
        var cacheKey = "database-browser:tables";
        if (!_cache.TryGetValue(cacheKey, out List<TableMetadataDto>? tables))
        {
            tables = await _browserService.GetTablesAsync(cancellationToken);

            _cache.Set(cacheKey, tables, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        return Ok(tables);
    }

    /// <summary>
    /// Get detailed schema information for a specific table
    /// </summary>
    /// <param name="tableName">Name of the table</param>
    /// <returns>Table schema with columns, keys, and relationships</returns>
    [HttpGet("tables/{tableName}/schema")]
    [ProducesResponseType(typeof(TableSchemaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TableSchemaDto>> GetTableSchema(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {User} requesting schema for table: {TableName}",
            User.Identity?.Name, tableName);

        // Cache schema for 10 minutes (schema is static)
        var cacheKey = $"database-browser:schema:{tableName}";
        if (!_cache.TryGetValue(cacheKey, out TableSchemaDto? schema))
        {
            schema = await _browserService.GetTableSchemaAsync(tableName, cancellationToken);

            if (schema == null)
            {
                return NotFound(new { message = $"Table '{tableName}' not found or not accessible" });
            }

            _cache.Set(cacheKey, schema, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        return Ok(schema);
    }

    /// <summary>
    /// Get paginated data from a table with optional sorting and searching
    /// </summary>
    /// <param name="tableName">Name of the table</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of records per page (max 1000)</param>
    /// <param name="sortBy">Column name to sort by</param>
    /// <param name="sortDirection">Sort direction: asc or desc</param>
    /// <param name="searchTerm">Global search term</param>
    /// <returns>Paginated table data</returns>
    [HttpGet("tables/{tableName}/data")]
    [ProducesResponseType(typeof(TableDataResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TableDataResultDto>> GetTableData(
        string tableName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = "asc",
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "User {User} requesting data from table: {TableName}, Page: {Page}, PageSize: {PageSize}",
            User.Identity?.Name, tableName, page, pageSize);

        // Validate parameters
        if (page < 1)
        {
            return BadRequest(new { message = "Page must be greater than 0" });
        }

        if (pageSize < 1 || pageSize > 1000)
        {
            return BadRequest(new { message = "PageSize must be between 1 and 1000" });
        }

        var request = new TableDataRequest
        {
            TableName = tableName,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection ?? "asc",
            SearchTerm = searchTerm
        };

        try
        {
            var result = await _browserService.GetTableDataAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for table: {TableName}", tableName);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching data from table: {TableName}", tableName);
            return StatusCode(500, new { message = "An error occurred while fetching table data" });
        }
    }

    /// <summary>
    /// Query table data with advanced filtering
    /// </summary>
    /// <param name="tableName">Name of the table</param>
    /// <param name="request">Query request with filters</param>
    /// <returns>Filtered and paginated table data</returns>
    [HttpPost("tables/{tableName}/query")]
    [ProducesResponseType(typeof(TableDataResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TableDataResultDto>> QueryTableData(
        string tableName,
        [FromBody] TableDataRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "User {User} querying table: {TableName} with {FilterCount} filters",
            User.Identity?.Name, tableName, request.Filters?.Count ?? 0);

        // Override table name from route
        request.TableName = tableName;

        // Validate
        if (request.Page < 1)
        {
            return BadRequest(new { message = "Page must be greater than 0" });
        }

        if (request.PageSize < 1 || request.PageSize > 1000)
        {
            return BadRequest(new { message = "PageSize must be between 1 and 1000" });
        }

        try
        {
            var result = await _browserService.GetTableDataAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid query for table: {TableName}", tableName);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying table: {TableName}", tableName);
            return StatusCode(500, new { message = "An error occurred while querying table data" });
        }
    }

    /// <summary>
    /// Get overall database statistics
    /// </summary>
    /// <returns>Database statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(DatabaseStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DatabaseStatisticsDto>> GetStatistics(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {User} requesting database statistics", User.Identity?.Name);

        var stats = await _browserService.GetDatabaseStatisticsAsync(cancellationToken);
        return Ok(stats);
    }
}
```

**Verification:**
```powershell
cd D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Api
dotnet build
```

**Test with Swagger:**
```powershell
dotnet run
```

Navigate to: `https://localhost:7001/swagger`

Look for the **DatabaseBrowser** endpoints.

---

## Phase 1 Complete! ✅

At this point, you have:
- ✅ All DTOs created
- ✅ Service interface defined
- ✅ Complete service implementation
- ✅ API controller with 5 endpoints
- ✅ Dependency injection configured
- ✅ Caching implemented

**Next: Phase 2 - Frontend Implementation**

Would you like me to continue with Phase 2 (Blazor frontend), or would you like to test Phase 1 first?

---

## Testing Phase 1 (Optional but Recommended)

### Test Endpoints with curl or Postman

1. **Start the API:**
```powershell
cd D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Api
dotnet run
```

2. **Get auth token (if you have authentication):**
```powershell
$response = Invoke-RestMethod -Uri "https://localhost:7001/api/auth/login" `
    -Method POST `
    -Body (@{email="admin@restaurant.com"; password="YourAdminPassword"; rememberMe=$false} | ConvertTo-Json) `
    -ContentType "application/json"

$token = $response.accessToken
```

3. **Test Get Tables:**
```powershell
Invoke-RestMethod -Uri "https://localhost:7001/api/admin/database/tables" `
    -Method GET `
    -Headers @{Authorization="Bearer $token"}
```

4. **Test Get Table Schema:**
```powershell
Invoke-RestMethod -Uri "https://localhost:7001/api/admin/database/tables/Users/schema" `
    -Method GET `
    -Headers @{Authorization="Bearer $token"}
```

5. **Test Get Table Data:**
```powershell
Invoke-RestMethod -Uri "https://localhost:7001/api/admin/database/tables/Users/data?page=1&pageSize=10" `
    -Method GET `
    -Headers @{Authorization="Bearer $token"}
```

---

**Continue to Phase 2 when ready!**
