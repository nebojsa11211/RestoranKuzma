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
