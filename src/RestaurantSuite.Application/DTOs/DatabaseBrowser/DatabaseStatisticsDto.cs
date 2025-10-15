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
