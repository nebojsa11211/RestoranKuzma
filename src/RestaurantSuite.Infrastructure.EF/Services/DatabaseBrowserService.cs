using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestaurantSuite.Application.DTOs.DatabaseBrowser;
using RestaurantSuite.Application.Interfaces;
using System.Linq.Dynamic.Core;
using System.Text;

namespace RestaurantSuite.Infrastructure.EF.Services;

/// <summary>
/// Service for browsing database metadata and data using EF Core's metadata API
/// Implements caching for schema information to improve performance
/// </summary>
public class DatabaseBrowserService : IDatabaseBrowserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DatabaseBrowserService> _logger;

    // Cache keys
    private const string TABLES_CACHE_KEY = "DatabaseBrowser_Tables";
    private const string SCHEMA_CACHE_KEY_PREFIX = "DatabaseBrowser_Schema_";

    // Security: List of tables that should NOT be exposed through the browser
    private static readonly HashSet<string> BlacklistedTables = new(StringComparer.OrdinalIgnoreCase)
    {
        // Add any tables that contain sensitive data that shouldn't be browsable
        // For example: "__EFMigrationsHistory"
        "__EFMigrationsHistory"
    };

    // Security: List of columns that should be redacted in responses
    private static readonly HashSet<string> SensitiveColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "PasswordHash",
        "Password",
        "RefreshToken",
        "Secret",
        "ApiKey",
        "Salt"
    };

    public DatabaseBrowserService(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<DatabaseBrowserService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<DatabaseTableInfo>> GetTablesAsync()
    {
        try
        {
            // Try to get from cache first
            if (_cache.TryGetValue(TABLES_CACHE_KEY, out List<DatabaseTableInfo>? cachedTables) && cachedTables != null)
            {
                return cachedTables;
            }

            var tables = new List<DatabaseTableInfo>();

            // Get all entity types from the EF Core model
            var entityTypes = _context.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var tableName = entityType.GetTableName();
                if (string.IsNullOrEmpty(tableName))
                    continue;

                // Skip blacklisted tables
                if (BlacklistedTables.Contains(tableName))
                    continue;

                var tableInfo = new DatabaseTableInfo
                {
                    TableName = tableName,
                    DisplayName = FormatDisplayName(tableName),
                    ColumnCount = entityType.GetProperties().Count(),
                    EstimatedRowCount = await GetEstimatedRowCountAsync(tableName),
                    IsAccessible = true
                };

                tables.Add(tableInfo);
            }

            // Cache for 5 minutes
            _cache.Set(TABLES_CACHE_KEY, tables, TimeSpan.FromMinutes(5));

            return tables.OrderBy(t => t.DisplayName).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving table list");
            throw;
        }
    }

    public Task<TableSchemaResponse?> GetTableSchemaAsync(string tableName)
    {
        try
        {
            if (!IsValidTableName(tableName))
            {
                _logger.LogWarning("Invalid or blacklisted table name requested: {TableName}", tableName);
                return Task.FromResult<TableSchemaResponse?>(null);
            }

            // Try to get from cache first
            var cacheKey = $"{SCHEMA_CACHE_KEY_PREFIX}{tableName.ToLowerInvariant()}";
            if (_cache.TryGetValue(cacheKey, out TableSchemaResponse? cachedSchema) && cachedSchema != null)
            {
                return Task.FromResult<TableSchemaResponse?>(cachedSchema);
            }

            // Find the entity type in the EF Core model
            var entityType = FindEntityType(tableName);
            if (entityType == null)
            {
                _logger.LogWarning("Entity type not found for table: {TableName}", tableName);
                return Task.FromResult<TableSchemaResponse?>(null);
            }

            var actualTableName = entityType.GetTableName() ?? tableName;

            var schema = new TableSchemaResponse
            {
                TableName = actualTableName,
                DisplayName = FormatDisplayName(actualTableName)
            };

            // Get columns
            var properties = entityType.GetProperties().OrderBy(p => p.GetColumnOrder() ?? int.MaxValue);
            int ordinal = 0;

            foreach (var property in properties)
            {
                var columnName = property.GetColumnName();
                var isPrimaryKey = property.IsPrimaryKey();
                var isForeignKey = property.IsForeignKey();

                var columnInfo = new TableColumnInfo
                {
                    ColumnName = columnName,
                    ClrType = property.ClrType.Name,
                    DatabaseType = property.GetColumnType() ?? "unknown",
                    IsNullable = property.IsNullable,
                    IsPrimaryKey = isPrimaryKey,
                    IsForeignKey = isForeignKey,
                    MaxLength = property.GetMaxLength(),
                    Precision = property.GetPrecision(),
                    Scale = property.GetScale(),
                    OrdinalPosition = ordinal++
                };

                schema.Columns.Add(columnInfo);

                if (isPrimaryKey)
                {
                    schema.PrimaryKeys.Add(columnName);
                }
            }

            // Get foreign keys
            var foreignKeys = entityType.GetForeignKeys();
            foreach (var fk in foreignKeys)
            {
                var principalTable = fk.PrincipalEntityType.GetTableName();
                if (principalTable == null) continue;

                var fkProperties = fk.Properties;
                var principalProperties = fk.PrincipalKey.Properties;

                for (int i = 0; i < fkProperties.Count; i++)
                {
                    var fkInfo = new ForeignKeyInfo
                    {
                        ColumnName = fkProperties[i].GetColumnName(),
                        ReferencedTable = principalTable,
                        ReferencedColumn = principalProperties[i].GetColumnName(),
                        ConstraintName = fk.GetConstraintName() ?? $"FK_{actualTableName}_{principalTable}"
                    };

                    schema.ForeignKeys.Add(fkInfo);
                }
            }

            // Get indexes
            var indexes = entityType.GetIndexes();
            foreach (var index in indexes)
            {
                // Determine if this index is for the primary key
                var isPrimaryKeyIndex = entityType.FindPrimaryKey()?.Properties
                    .Select(p => p.Name)
                    .SequenceEqual(index.Properties.Select(p => p.Name)) ?? false;

                var indexInfo = new IndexInfo
                {
                    IndexName = index.GetDatabaseName() ?? "Unknown",
                    Columns = index.Properties.Select(p => p.GetColumnName()).ToList(),
                    IsUnique = index.IsUnique,
                    IsPrimaryKey = isPrimaryKeyIndex
                };

                schema.Indexes.Add(indexInfo);
            }

            // Cache for 10 minutes
            _cache.Set(cacheKey, schema, TimeSpan.FromMinutes(10));

            return Task.FromResult<TableSchemaResponse?>(schema);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving schema for table: {TableName}", tableName);
            throw;
        }
    }

    public async Task<TableDataResponse?> GetTableDataAsync(string tableName, TableDataRequest request)
    {
        try
        {
            if (!IsValidTableName(tableName))
            {
                _logger.LogWarning("Invalid or blacklisted table name requested: {TableName}", tableName);
                return null;
            }

            var entityType = FindEntityType(tableName);
            if (entityType == null)
            {
                _logger.LogWarning("Entity type not found for table: {TableName}", tableName);
                return null;
            }

            var actualTableName = entityType.GetTableName() ?? tableName;

            // Get the DbSet dynamically
            var dbSetProperty = _context.GetType().GetProperties()
                .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                     p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                     p.PropertyType.GetGenericArguments()[0] == entityType.ClrType);

            if (dbSetProperty == null)
            {
                _logger.LogWarning("DbSet not found for entity type: {EntityType}", entityType.ClrType.Name);
                return null;
            }

            var dbSet = dbSetProperty.GetValue(_context) as IQueryable<object>;
            if (dbSet == null)
            {
                _logger.LogWarning("Could not get IQueryable for table: {TableName}", tableName);
                return null;
            }

            // Apply filters
            dbSet = ApplyFilters(dbSet, entityType, request);

            // Get total count before pagination
            var totalRows = await dbSet.CountAsync();

            // Apply sorting
            dbSet = ApplySorting(dbSet, entityType, request);

            // Apply pagination
            var skip = (request.Page - 1) * request.PageSize;
            var pagedData = await dbSet.Skip(skip).Take(request.PageSize).ToListAsync();

            // Build response
            var response = new TableDataResponse
            {
                TableName = actualTableName,
                TotalRows = totalRows,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalRows / (double)request.PageSize)
            };

            // Get column information
            var properties = entityType.GetProperties().ToList();
            response.Columns = properties.Select(p => p.GetColumnName()).ToList();

            foreach (var property in properties)
            {
                response.ColumnTypes[property.GetColumnName()] = property.ClrType.Name;
            }

            // Convert entities to dictionaries
            foreach (var entity in pagedData)
            {
                var row = new Dictionary<string, object?>();

                foreach (var property in properties)
                {
                    var columnName = property.GetColumnName();
                    var value = property.PropertyInfo?.GetValue(entity);

                    // Redact sensitive columns
                    if (SensitiveColumns.Contains(columnName))
                    {
                        row[columnName] = "[REDACTED]";
                    }
                    else
                    {
                        // Convert value to JSON-friendly format
                        row[columnName] = ConvertToJsonFriendlyValue(value);
                    }
                }

                response.Rows.Add(row);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving data for table: {TableName}", tableName);
            throw;
        }
    }

    public bool IsValidTableName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return false;

        // Check if blacklisted
        if (BlacklistedTables.Contains(tableName))
            return false;

        // Check if the table exists in the EF Core model
        return FindEntityType(tableName) != null;
    }

    #region Private Helper Methods

    private IEntityType? FindEntityType(string tableName)
    {
        return _context.Model.GetEntityTypes()
            .FirstOrDefault(et => string.Equals(et.GetTableName(), tableName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<long> GetEstimatedRowCountAsync(string tableName)
    {
        try
        {
            // Use raw SQL to get row count efficiently
            // This is a simple COUNT(*) which may be slow for very large tables
            // For production, you might want to use database-specific optimizations
            var sql = $"SELECT COUNT(*) FROM \"{tableName}\"";

            var connection = _context.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not get row count for table: {TableName}", tableName);
            return 0;
        }
    }

    private IQueryable<object> ApplyFilters(IQueryable<object> query, IEntityType entityType, TableDataRequest request)
    {
        // Apply search term (searches across all string columns)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var stringProperties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(string))
                .ToList();

            if (stringProperties.Any())
            {
                var predicates = new List<string>();
                var searchTerm = request.SearchTerm.Trim().ToLower();

                foreach (var prop in stringProperties)
                {
                    var propertyName = prop.Name;
                    // Skip sensitive columns
                    if (!SensitiveColumns.Contains(prop.GetColumnName()))
                    {
                        predicates.Add($"{propertyName} != null && {propertyName}.ToLower().Contains(@0)");
                    }
                }

                if (predicates.Any())
                {
                    var whereClause = string.Join(" || ", predicates);
                    query = query.Where(whereClause, searchTerm);
                }
            }
        }

        // Apply column-specific filters
        if (request.Filters != null && request.Filters.Any())
        {
            foreach (var filter in request.Filters)
            {
                var property = entityType.GetProperties()
                    .FirstOrDefault(p => string.Equals(p.GetColumnName(), filter.ColumnName, StringComparison.OrdinalIgnoreCase));

                if (property == null)
                    continue;

                // Skip sensitive columns
                if (SensitiveColumns.Contains(property.GetColumnName()))
                    continue;

                query = ApplyColumnFilter(query, property, filter);
            }
        }

        return query;
    }

    private IQueryable<object> ApplyColumnFilter(IQueryable<object> query, IProperty property, ColumnFilter filter)
    {
        var propertyName = property.Name;
        var propertyType = property.ClrType;

        switch (filter.Operator.ToLower())
        {
            case FilterOperators.Equals:
                if (filter.Value != null)
                {
                    var convertedValue = ConvertValue(filter.Value, propertyType);
                    query = query.Where($"{propertyName} == @0", convertedValue);
                }
                break;

            case FilterOperators.NotEquals:
                if (filter.Value != null)
                {
                    var convertedValue = ConvertValue(filter.Value, propertyType);
                    query = query.Where($"{propertyName} != @0", convertedValue);
                }
                break;

            case FilterOperators.Contains:
                if (propertyType == typeof(string) && filter.Value != null)
                {
                    query = query.Where($"{propertyName} != null && {propertyName}.Contains(@0)", filter.Value);
                }
                break;

            case FilterOperators.StartsWith:
                if (propertyType == typeof(string) && filter.Value != null)
                {
                    query = query.Where($"{propertyName} != null && {propertyName}.StartsWith(@0)", filter.Value);
                }
                break;

            case FilterOperators.EndsWith:
                if (propertyType == typeof(string) && filter.Value != null)
                {
                    query = query.Where($"{propertyName} != null && {propertyName}.EndsWith(@0)", filter.Value);
                }
                break;

            case FilterOperators.GreaterThan:
                if (filter.Value != null)
                {
                    var convertedValue = ConvertValue(filter.Value, propertyType);
                    query = query.Where($"{propertyName} > @0", convertedValue);
                }
                break;

            case FilterOperators.GreaterThanOrEqual:
                if (filter.Value != null)
                {
                    var convertedValue = ConvertValue(filter.Value, propertyType);
                    query = query.Where($"{propertyName} >= @0", convertedValue);
                }
                break;

            case FilterOperators.LessThan:
                if (filter.Value != null)
                {
                    var convertedValue = ConvertValue(filter.Value, propertyType);
                    query = query.Where($"{propertyName} < @0", convertedValue);
                }
                break;

            case FilterOperators.LessThanOrEqual:
                if (filter.Value != null)
                {
                    var convertedValue = ConvertValue(filter.Value, propertyType);
                    query = query.Where($"{propertyName} <= @0", convertedValue);
                }
                break;

            case FilterOperators.IsNull:
                query = query.Where($"{propertyName} == null");
                break;

            case FilterOperators.IsNotNull:
                query = query.Where($"{propertyName} != null");
                break;
        }

        return query;
    }

    private IQueryable<object> ApplySorting(IQueryable<object> query, IEntityType entityType, TableDataRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SortBy))
        {
            // Default: sort by primary key
            var primaryKey = entityType.FindPrimaryKey();
            if (primaryKey != null && primaryKey.Properties.Any())
            {
                var pkProperty = primaryKey.Properties.First();
                return query.OrderBy(pkProperty.Name);
            }
            return query;
        }

        // Find the property to sort by
        var property = entityType.GetProperties()
            .FirstOrDefault(p => string.Equals(p.GetColumnName(), request.SortBy, StringComparison.OrdinalIgnoreCase));

        if (property == null)
        {
            _logger.LogWarning("Sort column not found: {SortBy}", request.SortBy);
            return query;
        }

        var direction = request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        return query.OrderBy($"{property.Name} {direction}");
    }

    private object? ConvertValue(string value, Type targetType)
    {
        try
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType == typeof(Guid))
                return Guid.Parse(value);

            if (underlyingType == typeof(DateTime))
                return DateTime.Parse(value);

            if (underlyingType == typeof(DateTimeOffset))
                return DateTimeOffset.Parse(value);

            if (underlyingType == typeof(bool))
                return bool.Parse(value);

            if (underlyingType.IsEnum)
                return Enum.Parse(underlyingType, value, ignoreCase: true);

            return Convert.ChangeType(value, underlyingType);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error converting value '{Value}' to type {Type}", value, targetType.Name);
            return value;
        }
    }

    private object? ConvertToJsonFriendlyValue(object? value)
    {
        if (value == null)
            return null;

        var type = value.GetType();

        // Handle DateTime to ISO 8601 string
        if (type == typeof(DateTime))
            return ((DateTime)value).ToString("o");

        if (type == typeof(DateTimeOffset))
            return ((DateTimeOffset)value).ToString("o");

        // Handle Guid to string
        if (type == typeof(Guid))
            return value.ToString();

        // Handle enums to string
        if (type.IsEnum)
            return value.ToString();

        // Handle byte arrays to Base64
        if (type == typeof(byte[]))
            return Convert.ToBase64String((byte[])value);

        return value;
    }

    private string FormatDisplayName(string tableName)
    {
        // Convert "MenuItems" to "Menu Items"
        // Convert "OrderItemCustomizations" to "Order Item Customizations"
        if (string.IsNullOrWhiteSpace(tableName))
            return tableName;

        var result = new StringBuilder();
        result.Append(char.ToUpper(tableName[0]));

        for (int i = 1; i < tableName.Length; i++)
        {
            if (char.IsUpper(tableName[i]) && i > 0 && !char.IsUpper(tableName[i - 1]))
            {
                result.Append(' ');
            }
            result.Append(tableName[i]);
        }

        return result.ToString();
    }

    #endregion
}
