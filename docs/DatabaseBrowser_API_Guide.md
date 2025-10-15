# Database Browser API - Complete Guide

## Overview

The Database Browser API provides admin-only endpoints for inspecting and querying database structure and data. This is designed as a powerful diagnostic and development tool.

## Security

- **Authentication Required**: All endpoints require a valid JWT token
- **Authorization Required**: User must have the `Admin` role
- **Data Protection**: Sensitive columns (passwords, tokens, secrets) are automatically redacted
- **Table Whitelisting**: Only tables defined in the EF Core model are accessible
- **SQL Injection Prevention**: All queries use EF Core's parameterized queries

## Endpoints

### 1. Get All Tables

**GET** `/api/admin/database/tables`

Returns a list of all accessible database tables with metadata.

**Response:**
```json
[
  {
    "tableName": "Users",
    "displayName": "Users",
    "estimatedRowCount": 150,
    "columnCount": 12,
    "isAccessible": true
  },
  {
    "tableName": "Orders",
    "displayName": "Orders",
    "estimatedRowCount": 5423,
    "columnCount": 8,
    "isAccessible": true
  }
]
```

---

### 2. Get Table Schema

**GET** `/api/admin/database/tables/{tableName}/schema`

Returns detailed schema information for a specific table.

**Example:** `/api/admin/database/tables/Users/schema`

**Response:**
```json
{
  "tableName": "Users",
  "displayName": "Users",
  "columns": [
    {
      "columnName": "Id",
      "clrType": "Guid",
      "databaseType": "TEXT",
      "isNullable": false,
      "isPrimaryKey": true,
      "isForeignKey": false,
      "maxLength": null,
      "precision": null,
      "scale": null,
      "ordinalPosition": 0
    },
    {
      "columnName": "Email",
      "clrType": "String",
      "databaseType": "TEXT",
      "isNullable": false,
      "isPrimaryKey": false,
      "isForeignKey": false,
      "maxLength": 256,
      "precision": null,
      "scale": null,
      "ordinalPosition": 1
    },
    {
      "columnName": "Role",
      "clrType": "UserRole",
      "databaseType": "INTEGER",
      "isNullable": false,
      "isPrimaryKey": false,
      "isForeignKey": false,
      "maxLength": null,
      "precision": null,
      "scale": null,
      "ordinalPosition": 8
    }
  ],
  "primaryKeys": ["Id"],
  "foreignKeys": [],
  "indexes": [
    {
      "indexName": "IX_Users_Email",
      "columns": ["Email"],
      "isUnique": true,
      "isPrimaryKey": false
    }
  ]
}
```

---

### 3. Get Table Data (Simple)

**GET** `/api/admin/database/tables/{tableName}/data`

Returns paginated data from a table with optional sorting and searching.

**Query Parameters:**
- `page` (int, default: 1) - Page number (1-based)
- `pageSize` (int, default: 50, max: 1000) - Rows per page
- `sortBy` (string, optional) - Column name to sort by
- `sortDirection` (string, default: "asc") - Sort direction: "asc" or "desc"
- `searchTerm` (string, optional) - Search across all string columns

**Example:**
```
GET /api/admin/database/tables/Orders/data?page=1&pageSize=50&sortBy=CreatedAt&sortDirection=desc&searchTerm=pending
```

**Response:**
```json
{
  "tableName": "Orders",
  "totalRows": 5423,
  "page": 1,
  "pageSize": 50,
  "totalPages": 109,
  "columns": ["Id", "TableId", "WaiterId", "Status", "TotalAmount", "CreatedAt"],
  "columnTypes": {
    "Id": "Guid",
    "TableId": "Guid",
    "WaiterId": "Guid",
    "Status": "OrderStatus",
    "TotalAmount": "Decimal",
    "CreatedAt": "DateTime"
  },
  "rows": [
    {
      "Id": "a3b5c7d9-...",
      "TableId": "f1e2d3c4-...",
      "WaiterId": "b2c3d4e5-...",
      "Status": "Pending",
      "TotalAmount": 125.50,
      "CreatedAt": "2025-10-14T10:30:00Z"
    }
  ],
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

### 4. Query Table Data (Advanced)

**POST** `/api/admin/database/tables/{tableName}/query`

Advanced querying with multiple column filters and operators.

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 50,
  "sortBy": "CreatedAt",
  "sortDirection": "desc",
  "searchTerm": "pending",
  "filters": [
    {
      "columnName": "Status",
      "operator": "equals",
      "value": "Pending"
    },
    {
      "columnName": "TotalAmount",
      "operator": "greaterThan",
      "value": "100"
    },
    {
      "columnName": "CreatedAt",
      "operator": "greaterThanOrEqual",
      "value": "2025-10-01T00:00:00Z"
    }
  ]
}
```

**Supported Operators:**
- `equals` - Exact match
- `notEquals` - Not equal to
- `contains` - String contains (case-sensitive)
- `startsWith` - String starts with
- `endsWith` - String ends with
- `greaterThan` - Greater than (numbers, dates)
- `greaterThanOrEqual` - Greater than or equal
- `lessThan` - Less than
- `lessThanOrEqual` - Less than or equal
- `isNull` - Value is NULL
- `isNotNull` - Value is not NULL

**Response:** Same format as simple GET endpoint

---

## Usage Examples

### Example 1: Browse All Tables

```bash
curl -X GET "https://localhost:5001/api/admin/database/tables" \
  -H "Authorization: Bearer YOUR_ADMIN_JWT_TOKEN"
```

### Example 2: Get Schema for Orders Table

```bash
curl -X GET "https://localhost:5001/api/admin/database/tables/Orders/schema" \
  -H "Authorization: Bearer YOUR_ADMIN_JWT_TOKEN"
```

### Example 3: Get Recent Orders

```bash
curl -X GET "https://localhost:5001/api/admin/database/tables/Orders/data?page=1&pageSize=20&sortBy=CreatedAt&sortDirection=desc" \
  -H "Authorization: Bearer YOUR_ADMIN_JWT_TOKEN"
```

### Example 4: Search Users by Email

```bash
curl -X GET "https://localhost:5001/api/admin/database/tables/Users/data?searchTerm=john@example.com" \
  -H "Authorization: Bearer YOUR_ADMIN_JWT_TOKEN"
```

### Example 5: Advanced Query - Pending Orders Over $100

```bash
curl -X POST "https://localhost:5001/api/admin/database/tables/Orders/query" \
  -H "Authorization: Bearer YOUR_ADMIN_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 50,
    "sortBy": "TotalAmount",
    "sortDirection": "desc",
    "filters": [
      {
        "columnName": "Status",
        "operator": "equals",
        "value": "Pending"
      },
      {
        "columnName": "TotalAmount",
        "operator": "greaterThan",
        "value": "100"
      }
    ]
  }'
```

---

## Performance Considerations

### Caching
- **Table List**: Cached for 5 minutes
- **Table Schema**: Cached for 10 minutes
- Cache is automatically invalidated when the application restarts

### Pagination
- Maximum page size: 1000 rows
- Recommended page size: 50-100 rows for optimal performance
- For large tables (>100k rows), consider using filters to reduce result sets

### Query Optimization
- Queries use EF Core's IQueryable for efficient database queries
- Sorting and filtering are translated to SQL WHERE/ORDER BY clauses
- Only requested page data is loaded into memory

---

## Security Best Practices

### 1. Sensitive Data Redaction
The following column names are automatically redacted:
- PasswordHash
- Password
- RefreshToken
- Secret
- ApiKey
- Salt

Values are replaced with `[REDACTED]` in responses.

### 2. Table Access Control
The following tables are blacklisted and cannot be accessed:
- `__EFMigrationsHistory`

To add more blacklisted tables, update `DatabaseBrowserService.BlacklistedTables`.

### 3. Role-Based Access
- Only users with the `Admin` role can access these endpoints
- All requests are logged with user identification
- Failed authorization attempts are logged as warnings

### 4. SQL Injection Prevention
- All user input is parameterized using EF Core
- Table names are validated against the EF Core model
- Column names are validated against entity metadata
- No raw SQL is executed with user input

---

## Error Handling

### 400 Bad Request
```json
{
  "success": false,
  "message": "Invalid or inaccessible table name"
}
```

### 401 Unauthorized
User is not authenticated or token is invalid/expired.

### 403 Forbidden
User does not have the Admin role.

### 404 Not Found
```json
{
  "success": false,
  "message": "Table 'NonExistentTable' not found"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "An error occurred while retrieving the table data"
}
```

---

## Extending the Service

### Adding Custom Tables to Blacklist

Edit `DatabaseBrowserService.cs`:

```csharp
private static readonly HashSet<string> BlacklistedTables = new(StringComparer.OrdinalIgnoreCase)
{
    "__EFMigrationsHistory",
    "AuditLogs",  // Add your tables here
    "SystemConfig"
};
```

### Adding Custom Sensitive Columns

Edit `DatabaseBrowserService.cs`:

```csharp
private static readonly HashSet<string> SensitiveColumns = new(StringComparer.OrdinalIgnoreCase)
{
    "PasswordHash",
    "Password",
    "RefreshToken",
    "Secret",
    "ApiKey",
    "Salt",
    "SSN",           // Add your columns here
    "CreditCard"
};
```

---

## Integration with Frontend

### React/TypeScript Example

```typescript
interface TableDataResponse {
  tableName: string;
  totalRows: number;
  page: number;
  pageSize: number;
  totalPages: number;
  columns: string[];
  columnTypes: Record<string, string>;
  rows: Record<string, any>[];
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

async function fetchTableData(
  tableName: string,
  page: number = 1,
  pageSize: number = 50
): Promise<TableDataResponse> {
  const response = await fetch(
    `/api/admin/database/tables/${tableName}/data?page=${page}&pageSize=${pageSize}`,
    {
      headers: {
        'Authorization': `Bearer ${getAccessToken()}`,
        'Content-Type': 'application/json'
      }
    }
  );

  if (!response.ok) {
    throw new Error(`Failed to fetch table data: ${response.statusText}`);
  }

  return await response.json();
}
```

---

## Testing

### Unit Tests

See `DatabaseBrowserServiceTests.cs` for examples of:
- Table enumeration tests
- Schema retrieval tests
- Pagination tests
- Filtering and sorting tests
- Security tests (blacklist, redaction)

### Integration Tests

See `DatabaseBrowserControllerTests.cs` for examples of:
- Authorization tests
- End-to-end query tests
- Error handling tests

---

## Troubleshooting

### Issue: "Table not found"
**Solution**: Ensure the table is defined as a `DbSet<>` in `ApplicationDbContext.cs`

### Issue: "Invalid or inaccessible table name"
**Solution**: Check if the table is in the blacklist or not part of the EF Core model

### Issue: Slow queries on large tables
**Solution**:
- Use smaller page sizes
- Add filters to reduce result sets
- Ensure proper database indexes exist
- Consider adding database-specific optimizations for row counts

### Issue: Sensitive data still visible
**Solution**: Add column names to the `SensitiveColumns` set in `DatabaseBrowserService.cs`

---

## API Versioning

Current version: **v1**

Future versions may include:
- Bulk export functionality
- Data modification endpoints (UPDATE/DELETE)
- Custom SQL query execution (with additional security measures)
- Real-time data change notifications via SignalR
