using System.ComponentModel.DataAnnotations;

namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Request model for fetching paginated table data
/// </summary>
public class TableDataRequest
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size (number of rows per page)
    /// </summary>
    [Range(1, 1000)]
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// Column to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Search term (searches across all string columns)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Advanced filters for specific columns
    /// </summary>
    public List<ColumnFilter>? Filters { get; set; }
}

/// <summary>
/// Filter for a specific column
/// </summary>
public class ColumnFilter
{
    /// <summary>
    /// Column name to filter
    /// </summary>
    [Required]
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Filter operator (equals, contains, startsWith, endsWith, greaterThan, lessThan, etc.)
    /// </summary>
    [Required]
    public string Operator { get; set; } = "equals";

    /// <summary>
    /// Value to filter by
    /// </summary>
    public string? Value { get; set; }
}

/// <summary>
/// Supported filter operators
/// </summary>
public static class FilterOperators
{
    public new const string Equals = "equals";
    public const string NotEquals = "notEquals";
    public const string Contains = "contains";
    public const string StartsWith = "startsWith";
    public const string EndsWith = "endsWith";
    public const string GreaterThan = "greaterThan";
    public const string GreaterThanOrEqual = "greaterThanOrEqual";
    public const string LessThan = "lessThan";
    public const string LessThanOrEqual = "lessThanOrEqual";
    public const string IsNull = "isNull";
    public const string IsNotNull = "isNotNull";
    public const string In = "in"; // Value should be comma-separated
}
