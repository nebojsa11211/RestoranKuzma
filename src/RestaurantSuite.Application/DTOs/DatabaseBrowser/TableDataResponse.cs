namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Paginated response containing table data
/// </summary>
public class TableDataResponse
{
    /// <summary>
    /// Table name
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Total number of rows (without pagination)
    /// </summary>
    public long TotalRows { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Column names in order
    /// </summary>
    public List<string> Columns { get; set; } = new();

    /// <summary>
    /// Column types (CLR types)
    /// </summary>
    public Dictionary<string, string> ColumnTypes { get; set; } = new();

    /// <summary>
    /// The actual data rows (each row is a dictionary of column name to value)
    /// Values are serialized as strings for consistent JSON representation
    /// </summary>
    public List<Dictionary<string, object?>> Rows { get; set; } = new();

    /// <summary>
    /// Indicates if there's a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Indicates if there's a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
