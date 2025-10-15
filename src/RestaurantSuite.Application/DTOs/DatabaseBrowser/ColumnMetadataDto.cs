namespace RestaurantSuite.Application.DTOs.DatabaseBrowser;

/// <summary>
/// Represents metadata about a single column in a table
/// </summary>
public class ColumnMetadataDto
{
    /// <summary>
    /// Physical column name in database
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// CLR property name (may differ from column name)
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Data type (e.g., "String", "Int32", "DateTime", "Guid")
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// SQL database type (e.g., "varchar(100)", "int", "datetime2")
    /// </summary>
    public string? SqlDataType { get; set; }

    /// <summary>
    /// Whether column allows NULL values
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Whether this is a primary key column
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Whether this is a foreign key column
    /// </summary>
    public bool IsForeignKey { get; set; }

    /// <summary>
    /// Maximum length for string columns
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Whether this column contains sensitive data
    /// </summary>
    public bool IsSensitive { get; set; }

    /// <summary>
    /// Column ordinal position
    /// </summary>
    public int OrdinalPosition { get; set; }

    /// <summary>
    /// If foreign key, the related table name
    /// </summary>
    public string? ForeignKeyTable { get; set; }
}
