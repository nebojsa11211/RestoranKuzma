using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.DTOs.DatabaseBrowser;
using RestaurantSuite.Application.Interfaces;

namespace RestaurantSuite.Api.Controllers;

/// <summary>
/// Admin-only database browser for inspecting database structure and data
/// </summary>
[ApiController]
[Route("api/admin/database")]
[Authorize(Roles = "Admin")] // CRITICAL: Only admins can access this
public class DatabaseBrowserController : ControllerBase
{
    private readonly IDatabaseBrowserService _databaseBrowserService;
    private readonly ILogger<DatabaseBrowserController> _logger;

    public DatabaseBrowserController(
        IDatabaseBrowserService databaseBrowserService,
        ILogger<DatabaseBrowserController> logger)
    {
        _databaseBrowserService = databaseBrowserService;
        _logger = logger;
    }

    /// <summary>
    /// Get a list of all accessible database tables
    /// </summary>
    /// <remarks>
    /// Returns metadata about each table including name, row count, and column count.
    /// Only tables defined in the EF Core model are returned.
    /// </remarks>
    /// <response code="200">Returns the list of tables</response>
    /// <response code="401">Unauthorized - user is not authenticated</response>
    /// <response code="403">Forbidden - user is not an admin</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("tables")]
    [ProducesResponseType(typeof(List<DatabaseTableInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<DatabaseTableInfo>>> GetTables()
    {
        try
        {
            _logger.LogInformation("Admin user {UserId} requested table list", User.Identity?.Name);

            var tables = await _databaseBrowserService.GetTablesAsync();
            return Ok(tables);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving table list");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "An error occurred while retrieving the table list"
            });
        }
    }

    /// <summary>
    /// Get schema information for a specific table
    /// </summary>
    /// <param name="tableName">The name of the table (case-insensitive)</param>
    /// <remarks>
    /// Returns detailed schema information including columns, data types, primary keys,
    /// foreign keys, and indexes.
    /// </remarks>
    /// <response code="200">Returns the table schema</response>
    /// <response code="400">Bad request - invalid table name</response>
    /// <response code="401">Unauthorized - user is not authenticated</response>
    /// <response code="403">Forbidden - user is not an admin</response>
    /// <response code="404">Not found - table does not exist</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("tables/{tableName}/schema")]
    [ProducesResponseType(typeof(TableSchemaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TableSchemaResponse>> GetTableSchema(string tableName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Table name is required"
                });
            }

            // Validate table name to prevent injection attacks
            if (!_databaseBrowserService.IsValidTableName(tableName))
            {
                _logger.LogWarning("Admin user {UserId} attempted to access invalid table: {TableName}",
                    User.Identity?.Name, tableName);

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid or inaccessible table name"
                });
            }

            _logger.LogInformation("Admin user {UserId} requested schema for table: {TableName}",
                User.Identity?.Name, tableName);

            var schema = await _databaseBrowserService.GetTableSchemaAsync(tableName);

            if (schema == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Table '{tableName}' not found"
                });
            }

            return Ok(schema);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving schema for table: {TableName}", tableName);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "An error occurred while retrieving the table schema"
            });
        }
    }

    /// <summary>
    /// Get paginated data from a specific table
    /// </summary>
    /// <param name="tableName">The name of the table (case-insensitive)</param>
    /// <param name="page">Page number (1-based, default: 1)</param>
    /// <param name="pageSize">Number of rows per page (default: 50, max: 1000)</param>
    /// <param name="sortBy">Column name to sort by (optional)</param>
    /// <param name="sortDirection">Sort direction: 'asc' or 'desc' (default: 'asc')</param>
    /// <param name="searchTerm">Search term to filter across all string columns (optional)</param>
    /// <remarks>
    /// Returns paginated data from the specified table with optional filtering and sorting.
    /// Sensitive columns (passwords, tokens, etc.) are automatically redacted.
    ///
    /// Sample request:
    ///
    ///     GET /api/admin/database/tables/Users/data?page=1&amp;pageSize=50&amp;sortBy=CreatedAt&amp;sortDirection=desc
    ///
    /// </remarks>
    /// <response code="200">Returns the paginated table data</response>
    /// <response code="400">Bad request - invalid parameters or table name</response>
    /// <response code="401">Unauthorized - user is not authenticated</response>
    /// <response code="403">Forbidden - user is not an admin</response>
    /// <response code="404">Not found - table does not exist</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("tables/{tableName}/data")]
    [ProducesResponseType(typeof(TableDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TableDataResponse>> GetTableData(
        string tableName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "asc",
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Table name is required"
                });
            }

            // Validate table name
            if (!_databaseBrowserService.IsValidTableName(tableName))
            {
                _logger.LogWarning("Admin user {UserId} attempted to access invalid table: {TableName}",
                    User.Identity?.Name, tableName);

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid or inaccessible table name"
                });
            }

            // Validate pagination parameters
            if (page < 1)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Page number must be at least 1"
                });
            }

            if (pageSize < 1 || pageSize > 1000)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Page size must be between 1 and 1000"
                });
            }

            var request = new TableDataRequest
            {
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection,
                SearchTerm = searchTerm
            };

            _logger.LogInformation(
                "Admin user {UserId} requested data from table: {TableName} (Page: {Page}, PageSize: {PageSize})",
                User.Identity?.Name, tableName, page, pageSize);

            var data = await _databaseBrowserService.GetTableDataAsync(tableName, request);

            if (data == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Table '{tableName}' not found"
                });
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving data from table: {TableName}", tableName);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "An error occurred while retrieving the table data"
            });
        }
    }

    /// <summary>
    /// Get table data with advanced filtering
    /// </summary>
    /// <param name="tableName">The name of the table (case-insensitive)</param>
    /// <param name="request">Advanced query request with filters, sorting, and pagination</param>
    /// <remarks>
    /// Provides advanced querying capabilities with support for multiple column filters,
    /// comparison operators, and combined search criteria.
    ///
    /// Sample request body:
    ///
    ///     POST /api/admin/database/tables/Orders/query
    ///     {
    ///       "page": 1,
    ///       "pageSize": 50,
    ///       "sortBy": "CreatedAt",
    ///       "sortDirection": "desc",
    ///       "searchTerm": "pending",
    ///       "filters": [
    ///         {
    ///           "columnName": "Status",
    ///           "operator": "equals",
    ///           "value": "Pending"
    ///         },
    ///         {
    ///           "columnName": "TotalAmount",
    ///           "operator": "greaterThan",
    ///           "value": "100"
    ///         }
    ///       ]
    ///     }
    ///
    /// Supported operators: equals, notEquals, contains, startsWith, endsWith,
    /// greaterThan, greaterThanOrEqual, lessThan, lessThanOrEqual, isNull, isNotNull
    /// </remarks>
    /// <response code="200">Returns the filtered table data</response>
    /// <response code="400">Bad request - invalid parameters, filters, or table name</response>
    /// <response code="401">Unauthorized - user is not authenticated</response>
    /// <response code="403">Forbidden - user is not an admin</response>
    /// <response code="404">Not found - table does not exist</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("tables/{tableName}/query")]
    [ProducesResponseType(typeof(TableDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TableDataResponse>> QueryTableData(
        string tableName,
        [FromBody] TableDataRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Table name is required"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request data",
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            // Validate table name
            if (!_databaseBrowserService.IsValidTableName(tableName))
            {
                _logger.LogWarning("Admin user {UserId} attempted to access invalid table: {TableName}",
                    User.Identity?.Name, tableName);

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid or inaccessible table name"
                });
            }

            _logger.LogInformation(
                "Admin user {UserId} queried table: {TableName} with {FilterCount} filters",
                User.Identity?.Name, tableName, request.Filters?.Count ?? 0);

            var data = await _databaseBrowserService.GetTableDataAsync(tableName, request);

            if (data == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Table '{tableName}' not found"
                });
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying table: {TableName}", tableName);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                success = false,
                message = "An error occurred while querying the table"
            });
        }
    }
}
