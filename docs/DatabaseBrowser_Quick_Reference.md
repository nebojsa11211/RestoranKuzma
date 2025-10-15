# Database Browser API - Quick Reference

## Endpoints Cheat Sheet

| Method | Endpoint | Purpose | Auth Required |
|--------|----------|---------|---------------|
| GET | `/api/admin/database/tables` | List all tables | Admin |
| GET | `/api/admin/database/tables/{tableName}/schema` | Get table schema | Admin |
| GET | `/api/admin/database/tables/{tableName}/data` | Get table data (simple) | Admin |
| POST | `/api/admin/database/tables/{tableName}/query` | Query with filters | Admin |

---

## Quick Start

### 1. Get Admin Token

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@restaurant.com","password":"Admin123!","rememberMe":false}'
```

### 2. List Tables

```bash
curl -X GET http://localhost:5000/api/admin/database/tables \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### 3. Get Table Data

```bash
curl -X GET "http://localhost:5000/api/admin/database/tables/Users/data?page=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Request Parameters

### TableDataRequest (Query String or Body)

| Parameter | Type | Default | Max | Description |
|-----------|------|---------|-----|-------------|
| `page` | int | 1 | - | Page number (1-based) |
| `pageSize` | int | 50 | 1000 | Rows per page |
| `sortBy` | string | null | - | Column to sort by |
| `sortDirection` | string | "asc" | - | "asc" or "desc" |
| `searchTerm` | string | null | - | Search across string columns |
| `filters` | array | null | - | Advanced column filters |

### Filter Operators

| Operator | Types | Example |
|----------|-------|---------|
| `equals` | All | `"Status" equals "Pending"` |
| `notEquals` | All | `"Role" notEquals "Guest"` |
| `contains` | String | `"Email" contains "@gmail.com"` |
| `startsWith` | String | `"Name" startsWith "John"` |
| `endsWith` | String | `"Email" endsWith ".com"` |
| `greaterThan` | Number, Date | `"TotalAmount" greaterThan "100"` |
| `greaterThanOrEqual` | Number, Date | `"CreatedAt" greaterThanOrEqual "2025-10-01"` |
| `lessThan` | Number, Date | `"Price" lessThan "50"` |
| `lessThanOrEqual` | Number, Date | `"Quantity" lessThanOrEqual "10"` |
| `isNull` | All | `"DeletedAt" isNull` |
| `isNotNull` | All | `"UpdatedAt" isNotNull` |

---

## Response Formats

### DatabaseTableInfo

```json
{
  "tableName": "Users",
  "displayName": "Users",
  "estimatedRowCount": 150,
  "columnCount": 12,
  "isAccessible": true
}
```

### TableSchemaResponse

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
    }
  ],
  "primaryKeys": ["Id"],
  "foreignKeys": [],
  "indexes": []
}
```

### TableDataResponse

```json
{
  "tableName": "Orders",
  "totalRows": 5423,
  "page": 1,
  "pageSize": 50,
  "totalPages": 109,
  "columns": ["Id", "Status", "TotalAmount"],
  "columnTypes": {
    "Id": "Guid",
    "Status": "OrderStatus",
    "TotalAmount": "Decimal"
  },
  "rows": [
    {
      "Id": "a3b5c7d9-...",
      "Status": "Pending",
      "TotalAmount": 125.50
    }
  ],
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

## Example Queries

### Get All Active Users

```bash
curl -X POST http://localhost:5000/api/admin/database/tables/Users/query \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 50,
    "filters": [
      {"columnName": "IsActive", "operator": "equals", "value": "true"}
    ]
  }'
```

### Get Orders Over $100

```bash
curl -X POST http://localhost:5000/api/admin/database/tables/Orders/query \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 50,
    "sortBy": "TotalAmount",
    "sortDirection": "desc",
    "filters": [
      {"columnName": "TotalAmount", "operator": "greaterThan", "value": "100"}
    ]
  }'
```

### Search Users by Email

```bash
curl -X GET "http://localhost:5000/api/admin/database/tables/Users/data?searchTerm=john@example.com" \
  -H "Authorization: Bearer TOKEN"
```

### Get Recent Orders (Last 7 Days)

```bash
curl -X POST http://localhost:5000/api/admin/database/tables/Orders/query \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 100,
    "sortBy": "CreatedAt",
    "sortDirection": "desc",
    "filters": [
      {
        "columnName": "CreatedAt",
        "operator": "greaterThanOrEqual",
        "value": "2025-10-07T00:00:00Z"
      }
    ]
  }'
```

### Complex Query: Pending Orders for Specific Waiter

```bash
curl -X POST http://localhost:5000/api/admin/database/tables/Orders/query \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 50,
    "sortBy": "CreatedAt",
    "sortDirection": "asc",
    "filters": [
      {"columnName": "Status", "operator": "equals", "value": "Pending"},
      {"columnName": "WaiterId", "operator": "equals", "value": "YOUR-WAITER-GUID"}
    ]
  }'
```

---

## HTTP Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Request successful |
| 400 | Bad Request | Invalid parameters or table name |
| 401 | Unauthorized | Not authenticated |
| 403 | Forbidden | Not an admin |
| 404 | Not Found | Table doesn't exist |
| 500 | Internal Server Error | Unexpected error |

---

## Common Error Responses

### Invalid Table Name
```json
{
  "success": false,
  "message": "Invalid or inaccessible table name"
}
```

### Table Not Found
```json
{
  "success": false,
  "message": "Table 'InvalidTable' not found"
}
```

### Invalid Page Size
```json
{
  "success": false,
  "message": "Page size must be between 1 and 1000"
}
```

---

## Security Notes

### Automatically Redacted Columns

These column names are automatically redacted in responses:
- `PasswordHash`
- `Password`
- `RefreshToken`
- `Secret`
- `ApiKey`
- `Salt`

Values are replaced with: `[REDACTED]`

### Blacklisted Tables

These tables cannot be accessed:
- `__EFMigrationsHistory`

---

## Testing Scripts

### PowerShell (Windows)

```powershell
# Run the test script
.\scripts\Test-DatabaseBrowserAPI.ps1 -BaseUrl "http://localhost:5000"
```

### Bash (Linux/Mac)

```bash
# Make executable
chmod +x scripts/test-database-browser.sh

# Run the test script
./scripts/test-database-browser.sh
```

---

## Performance Tips

1. **Use pagination**: Always specify reasonable page sizes (50-100 recommended)
2. **Add filters**: Reduce result sets with filters before paginating
3. **Cache results**: Schema information is cached for 10 minutes
4. **Sort wisely**: Sorting by indexed columns is faster
5. **Limit searches**: Avoid wildcard searches on very large tables

---

## Files Reference

### Key Implementation Files

| File | Purpose |
|------|---------|
| `DatabaseBrowserController.cs` | API endpoints |
| `DatabaseBrowserService.cs` | Core logic |
| `IDatabaseBrowserService.cs` | Service interface |
| `TableDataRequest.cs` | Request DTOs |
| `TableDataResponse.cs` | Response DTOs |

### Documentation Files

| File | Purpose |
|------|---------|
| `DatabaseBrowser_API_Guide.md` | Complete API documentation |
| `DatabaseBrowser_Implementation_Summary.md` | Architecture & design decisions |
| `DatabaseBrowser_Best_Practices.md` | Production best practices |
| `DatabaseBrowser_Quick_Reference.md` | This file |

### Test Scripts

| File | Purpose |
|------|---------|
| `Test-DatabaseBrowserAPI.ps1` | PowerShell test script |
| `test-database-browser.sh` | Bash test script |

---

## TypeScript Types

```typescript
interface DatabaseTableInfo {
  tableName: string;
  displayName: string;
  estimatedRowCount: number;
  columnCount: number;
  isAccessible: boolean;
}

interface TableColumnInfo {
  columnName: string;
  clrType: string;
  databaseType: string;
  isNullable: boolean;
  isPrimaryKey: boolean;
  isForeignKey: boolean;
  maxLength?: number;
  precision?: number;
  scale?: number;
  ordinalPosition: number;
}

interface TableSchemaResponse {
  tableName: string;
  displayName: string;
  columns: TableColumnInfo[];
  primaryKeys: string[];
  foreignKeys: ForeignKeyInfo[];
  indexes: IndexInfo[];
}

interface TableDataRequest {
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDirection: 'asc' | 'desc';
  searchTerm?: string;
  filters?: ColumnFilter[];
}

interface ColumnFilter {
  columnName: string;
  operator: FilterOperator;
  value?: string;
}

type FilterOperator =
  | 'equals'
  | 'notEquals'
  | 'contains'
  | 'startsWith'
  | 'endsWith'
  | 'greaterThan'
  | 'greaterThanOrEqual'
  | 'lessThan'
  | 'lessThanOrEqual'
  | 'isNull'
  | 'isNotNull';

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
```

---

## C# Usage Example

```csharp
// Inject the service
private readonly IDatabaseBrowserService _databaseBrowserService;

// Get all tables
var tables = await _databaseBrowserService.GetTablesAsync();

// Get schema
var schema = await _databaseBrowserService.GetTableSchemaAsync("Users");

// Query with filters
var request = new TableDataRequest
{
    Page = 1,
    PageSize = 50,
    SortBy = "CreatedAt",
    SortDirection = "desc",
    Filters = new List<ColumnFilter>
    {
        new ColumnFilter
        {
            ColumnName = "IsActive",
            Operator = "equals",
            Value = "true"
        }
    }
};

var data = await _databaseBrowserService.GetTableDataAsync("Users", request);
```

---

## Troubleshooting Quick Fixes

| Issue | Solution |
|-------|----------|
| 401 Unauthorized | Check JWT token is valid and not expired |
| 403 Forbidden | Ensure user has Admin role |
| Table not found | Verify table exists in ApplicationDbContext |
| Slow queries | Add database indexes, reduce page size |
| Cache issues | Restart application to clear cache |
| Sensitive data visible | Add column to `SensitiveColumns` set |

---

## Support & Resources

- **Full Documentation**: See `DatabaseBrowser_API_Guide.md`
- **Implementation Details**: See `DatabaseBrowser_Implementation_Summary.md`
- **Best Practices**: See `DatabaseBrowser_Best_Practices.md`
- **Test Scripts**: Located in `/scripts` directory
- **Swagger UI**: Available at `/swagger` when running in development mode

---

## Version

**Current Version**: 1.0.0
**Last Updated**: 2025-10-14
**API Version**: v1
