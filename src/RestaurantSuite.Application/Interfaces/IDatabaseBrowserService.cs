using RestaurantSuite.Application.DTOs.DatabaseBrowser;

namespace RestaurantSuite.Application.Interfaces;

/// <summary>
/// Service for browsing database metadata and data
/// Used for admin/development tools to inspect database contents
/// </summary>
public interface IDatabaseBrowserService
{
    /// <summary>
    /// Gets a list of all accessible tables in the database
    /// </summary>
    Task<List<DatabaseTableInfo>> GetTablesAsync();

    /// <summary>
    /// Gets schema information for a specific table
    /// </summary>
    /// <param name="tableName">Name of the table (case-insensitive)</param>
    Task<TableSchemaResponse?> GetTableSchemaAsync(string tableName);

    /// <summary>
    /// Gets paginated data from a specific table
    /// </summary>
    /// <param name="tableName">Name of the table (case-insensitive)</param>
    /// <param name="request">Pagination and filtering parameters</param>
    Task<TableDataResponse?> GetTableDataAsync(string tableName, TableDataRequest request);

    /// <summary>
    /// Validates if a table name is valid and accessible
    /// </summary>
    /// <param name="tableName">Name of the table to validate</param>
    bool IsValidTableName(string tableName);
}
