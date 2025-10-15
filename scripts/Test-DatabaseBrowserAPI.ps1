# Test-DatabaseBrowserAPI.ps1
# PowerShell script to test the Database Browser API endpoints

param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$AdminEmail = "admin@restaurant.com",
    [string]$AdminPassword = "Admin123!",
    [switch]$SkipLogin
)

$ErrorActionPreference = "Stop"

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Database Browser API Test Script" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Global variables
$Script:AccessToken = $null
$Script:Headers = @{
    "Content-Type" = "application/json"
}

# Function to login and get access token
function Get-AdminToken {
    Write-Host "Step 1: Authenticating as admin..." -ForegroundColor Yellow

    $loginBody = @{
        email = $AdminEmail
        password = $AdminPassword
        rememberMe = $false
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/auth/login" `
            -Method Post `
            -Body $loginBody `
            -ContentType "application/json"

        if ($response.success) {
            $Script:AccessToken = $response.accessToken
            $Script:Headers["Authorization"] = "Bearer $($Script:AccessToken)"
            Write-Host "  Success! Authenticated as: $($response.email) (Role: $($response.role))" -ForegroundColor Green
            Write-Host ""
            return $true
        } else {
            Write-Host "  Error: $($response.errorMessage)" -ForegroundColor Red
            return $false
        }
    } catch {
        Write-Host "  Error: Failed to authenticate. Make sure the API is running and credentials are correct." -ForegroundColor Red
        Write-Host "  Exception: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Function to test GET /api/admin/database/tables
function Test-GetTables {
    Write-Host "Step 2: Getting list of all tables..." -ForegroundColor Yellow

    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/admin/database/tables" `
            -Method Get `
            -Headers $Script:Headers

        Write-Host "  Found $($response.Count) tables:" -ForegroundColor Green

        foreach ($table in $response) {
            Write-Host "    - $($table.displayName) ($($table.tableName)): $($table.estimatedRowCount) rows, $($table.columnCount) columns" -ForegroundColor Cyan
        }

        Write-Host ""
        return $response
    } catch {
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        return $null
    }
}

# Function to test GET /api/admin/database/tables/{tableName}/schema
function Test-GetTableSchema {
    param([string]$TableName)

    Write-Host "Step 3: Getting schema for table '$TableName'..." -ForegroundColor Yellow

    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/admin/database/tables/$TableName/schema" `
            -Method Get `
            -Headers $Script:Headers

        Write-Host "  Table: $($response.displayName)" -ForegroundColor Green
        Write-Host "  Columns ($($response.columns.Count)):" -ForegroundColor Green

        foreach ($column in $response.columns) {
            $badges = @()
            if ($column.isPrimaryKey) { $badges += "[PK]" }
            if ($column.isForeignKey) { $badges += "[FK]" }
            if (-not $column.isNullable) { $badges += "[NOT NULL]" }

            $badgeStr = if ($badges.Count -gt 0) { " " + ($badges -join " ") } else { "" }
            Write-Host "    - $($column.columnName): $($column.clrType)$badgeStr" -ForegroundColor Cyan
        }

        if ($response.primaryKeys.Count -gt 0) {
            Write-Host "  Primary Keys: $($response.primaryKeys -join ', ')" -ForegroundColor Green
        }

        if ($response.foreignKeys.Count -gt 0) {
            Write-Host "  Foreign Keys:" -ForegroundColor Green
            foreach ($fk in $response.foreignKeys) {
                Write-Host "    - $($fk.columnName) -> $($fk.referencedTable).$($fk.referencedColumn)" -ForegroundColor Cyan
            }
        }

        if ($response.indexes.Count -gt 0) {
            Write-Host "  Indexes:" -ForegroundColor Green
            foreach ($index in $response.indexes) {
                $uniqueStr = if ($index.isUnique) { " [UNIQUE]" } else { "" }
                Write-Host "    - $($index.indexName): $($index.columns -join ', ')$uniqueStr" -ForegroundColor Cyan
            }
        }

        Write-Host ""
        return $response
    } catch {
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        return $null
    }
}

# Function to test GET /api/admin/database/tables/{tableName}/data
function Test-GetTableData {
    param(
        [string]$TableName,
        [int]$Page = 1,
        [int]$PageSize = 10,
        [string]$SortBy = $null,
        [string]$SortDirection = "asc",
        [string]$SearchTerm = $null
    )

    Write-Host "Step 4: Getting data from table '$TableName'..." -ForegroundColor Yellow

    # Build query parameters
    $queryParams = "?page=$Page&pageSize=$PageSize"
    if ($SortBy) { $queryParams += "&sortBy=$SortBy" }
    if ($SortDirection) { $queryParams += "&sortDirection=$SortDirection" }
    if ($SearchTerm) { $queryParams += "&searchTerm=$SearchTerm" }

    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/admin/database/tables/$TableName/data$queryParams" `
            -Method Get `
            -Headers $Script:Headers

        Write-Host "  Table: $($response.tableName)" -ForegroundColor Green
        Write-Host "  Total Rows: $($response.totalRows)" -ForegroundColor Green
        Write-Host "  Page: $($response.page) of $($response.totalPages)" -ForegroundColor Green
        Write-Host "  Showing $($response.rows.Count) rows:" -ForegroundColor Green
        Write-Host ""

        # Display rows in a table format
        if ($response.rows.Count -gt 0) {
            # Display column headers
            $headers = $response.columns -join " | "
            Write-Host "  $headers" -ForegroundColor Cyan
            Write-Host "  $('-' * $headers.Length)" -ForegroundColor Cyan

            # Display rows
            foreach ($row in $response.rows) {
                $rowValues = @()
                foreach ($column in $response.columns) {
                    $value = $row.$column
                    if ($null -eq $value) {
                        $rowValues += "[NULL]"
                    } elseif ($value -is [string] -and $value.Length -gt 30) {
                        $rowValues += "$($value.Substring(0, 27))..."
                    } else {
                        $rowValues += $value.ToString()
                    }
                }
                Write-Host "  $($rowValues -join ' | ')" -ForegroundColor White
            }
        }

        Write-Host ""
        return $response
    } catch {
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        return $null
    }
}

# Function to test POST /api/admin/database/tables/{tableName}/query
function Test-QueryTableData {
    param(
        [string]$TableName,
        [hashtable]$RequestBody
    )

    Write-Host "Step 5: Querying table '$TableName' with filters..." -ForegroundColor Yellow

    $body = $RequestBody | ConvertTo-Json -Depth 10

    try {
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/admin/database/tables/$TableName/query" `
            -Method Post `
            -Headers $Script:Headers `
            -Body $body

        Write-Host "  Table: $($response.tableName)" -ForegroundColor Green
        Write-Host "  Total Matching Rows: $($response.totalRows)" -ForegroundColor Green
        Write-Host "  Page: $($response.page) of $($response.totalPages)" -ForegroundColor Green
        Write-Host "  Returned $($response.rows.Count) rows" -ForegroundColor Green
        Write-Host ""

        # Display first few rows
        if ($response.rows.Count -gt 0) {
            $maxDisplay = [Math]::Min(5, $response.rows.Count)
            Write-Host "  First $maxDisplay rows:" -ForegroundColor Cyan

            for ($i = 0; $i -lt $maxDisplay; $i++) {
                $row = $response.rows[$i]
                Write-Host "    Row $($i + 1):" -ForegroundColor Yellow
                foreach ($column in $response.columns) {
                    $value = $row.$column
                    if ($null -eq $value) { $value = "[NULL]" }
                    Write-Host "      $column`: $value" -ForegroundColor White
                }
                Write-Host ""
            }
        }

        return $response
    } catch {
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails) {
            Write-Host "  Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
        Write-Host ""
        return $null
    }
}

# Main execution
function Main {
    Write-Host "API Base URL: $BaseUrl" -ForegroundColor Cyan
    Write-Host ""

    # Step 1: Login
    if (-not $SkipLogin) {
        if (-not (Get-AdminToken)) {
            Write-Host "Authentication failed. Exiting." -ForegroundColor Red
            return
        }
    } else {
        Write-Host "Skipping login (using existing token)" -ForegroundColor Yellow
        Write-Host ""
    }

    # Step 2: Get all tables
    $tables = Test-GetTables
    if (-not $tables -or $tables.Count -eq 0) {
        Write-Host "No tables found or error occurred. Exiting." -ForegroundColor Red
        return
    }

    # Step 3: Get schema for first table
    $firstTable = $tables[0].tableName
    $schema = Test-GetTableSchema -TableName $firstTable
    if (-not $schema) {
        Write-Host "Failed to get schema. Exiting." -ForegroundColor Red
        return
    }

    # Step 4: Get data from first table
    $data = Test-GetTableData -TableName $firstTable -Page 1 -PageSize 5
    if (-not $data) {
        Write-Host "Failed to get table data. Exiting." -ForegroundColor Red
        return
    }

    # Step 5: Try advanced query (if applicable)
    # Example: Query Users table for active users
    if ($tables | Where-Object { $_.tableName -eq "Users" }) {
        Write-Host "Testing advanced query on Users table..." -ForegroundColor Yellow

        $queryRequest = @{
            page = 1
            pageSize = 10
            sortBy = "CreatedAt"
            sortDirection = "desc"
            filters = @(
                @{
                    columnName = "IsActive"
                    operator = "equals"
                    value = "true"
                }
            )
        }

        $queryResult = Test-QueryTableData -TableName "Users" -RequestBody $queryRequest
    }

    Write-Host "================================" -ForegroundColor Green
    Write-Host "All tests completed successfully!" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Green
}

# Run the main function
Main
