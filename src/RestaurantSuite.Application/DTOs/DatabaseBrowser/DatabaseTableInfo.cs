namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Represents metadata information about a database table
/// </summary>
public class DatabaseTableInfo
{
    /// <summary>
    /// The name of the table
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// The display-friendly name (derived from table name)
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Estimated row count (may not be exact for large tables)
    /// </summary>
    public long EstimatedRowCount { get; set; }

    /// <summary>
    /// Number of columns in the table
    /// </summary>
    public int ColumnCount { get; set; }

    /// <summary>
    /// Whether this table is accessible through the browser
    /// </summary>
    public bool IsAccessible { get; set; } = true;
}
