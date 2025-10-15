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
