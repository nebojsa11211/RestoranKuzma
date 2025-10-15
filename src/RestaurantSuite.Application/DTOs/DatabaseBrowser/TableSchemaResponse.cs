namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Complete schema information for a database table
/// </summary>
public class TableSchemaResponse
{
    /// <summary>
    /// Name of the table
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Display-friendly name
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// List of columns with their metadata
    /// </summary>
    public List<TableColumnInfo> Columns { get; set; } = new();

    /// <summary>
    /// Names of primary key columns
    /// </summary>
    public List<string> PrimaryKeys { get; set; } = new();

    /// <summary>
    /// Foreign key relationships
    /// </summary>
    public List<ForeignKeyInfo> ForeignKeys { get; set; } = new();

    /// <summary>
    /// Indexes defined on the table
    /// </summary>
    public List<IndexInfo> Indexes { get; set; } = new();
}

/// <summary>
/// Foreign key relationship information
/// </summary>
public class ForeignKeyInfo
{
    public string ColumnName { get; set; } = string.Empty;
    public string ReferencedTable { get; set; } = string.Empty;
    public string ReferencedColumn { get; set; } = string.Empty;
    public string ConstraintName { get; set; } = string.Empty;
}

/// <summary>
/// Index information
/// </summary>
public class IndexInfo
{
    public string IndexName { get; set; } = string.Empty;
    public List<string> Columns { get; set; } = new();
    public bool IsUnique { get; set; }
    public bool IsPrimaryKey { get; set; }
}
