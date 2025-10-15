namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Represents schema information for a single database column
/// </summary>
public class TableColumnInfo
{
    /// <summary>
    /// Column name
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// .NET CLR type name (e.g., "String", "Int32", "DateTime")
    /// </summary>
    public string ClrType { get; set; } = string.Empty;

    /// <summary>
    /// Database-specific type name (e.g., "nvarchar", "int", "datetime2")
    /// </summary>
    public string DatabaseType { get; set; } = string.Empty;

    /// <summary>
    /// Whether the column is nullable
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Whether this column is part of the primary key
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Whether this column is a foreign key
    /// </summary>
    public bool IsForeignKey { get; set; }

    /// <summary>
    /// Maximum length (for string columns)
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Precision (for decimal columns)
    /// </summary>
    public int? Precision { get; set; }

    /// <summary>
    /// Scale (for decimal columns)
    /// </summary>
    public int? Scale { get; set; }

    /// <summary>
    /// Ordinal position in the table
    /// </summary>
    public int OrdinalPosition { get; set; }
}
