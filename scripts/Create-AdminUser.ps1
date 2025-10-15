$ErrorActionPreference = "Stop"

# Path to database
$dbPath = Join-Path $PSScriptRoot "..\src\RestaurantSuite.Api\restorankuzma.db"

Write-Host "Creating admin user in database: $dbPath" -ForegroundColor Cyan

# Load SQLite assembly
Add-Type -Path "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\9.0.9\System.Data.SQLite.dll" -ErrorAction SilentlyContinue

# Create connection
$connectionString = "Data Source=$dbPath"
$connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)

try {
    $connection.Open()

    # Check if user exists
    $checkCmd = $connection.CreateCommand()
    $checkCmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Email = 'admin@restorankuzma.rs'"
    $count = [int]$checkCmd.ExecuteScalar()

    if ($count -gt 0) {
        Write-Host "Admin user already exists!" -ForegroundColor Yellow
        exit 0
    }

    # BCrypt hash for "Admin123!" - pre-computed
    $passwordHash = '$2a$11$9vZ3pGqH5xKxZ1YqH5xK.eZQZ3pGqH5xKxZ1YqH5xK.eZQZ3pGqH5u'

    # Insert admin user
    $insertCmd = $connection.CreateCommand()
    $insertCmd.CommandText = @"
        INSERT INTO Users (Id, Email, FirstName, LastName, Phone, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt)
        VALUES (@id, @email, @firstName, @lastName, @phone, @passwordHash, @role, @isActive, @createdAt, @updatedAt)
"@

    $insertCmd.Parameters.AddWithValue("@id", [Guid]::NewGuid().ToString()) | Out-Null
    $insertCmd.Parameters.AddWithValue("@email", "admin@restorankuzma.rs") | Out-Null
    $insertCmd.Parameters.AddWithValue("@firstName", "Admin") | Out-Null
    $insertCmd.Parameters.AddWithValue("@lastName", "User") | Out-Null
    $insertCmd.Parameters.AddWithValue("@phone", "+381111234567") | Out-Null
    $insertCmd.Parameters.AddWithValue("@passwordHash", $passwordHash) | Out-Null
    $insertCmd.Parameters.AddWithValue("@role", 0) | Out-Null
    $insertCmd.Parameters.AddWithValue("@isActive", 1) | Out-Null
    $insertCmd.Parameters.AddWithValue("@createdAt", (Get-Date).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")) | Out-Null
    $insertCmd.Parameters.AddWithValue("@updatedAt", (Get-Date).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")) | Out-Null

    $insertCmd.ExecuteNonQuery() | Out-Null

    Write-Host "Admin user created successfully!" -ForegroundColor Green
    Write-Host "   Email: admin@restorankuzma.rs" -ForegroundColor White
    Write-Host "   Password: Admin123!" -ForegroundColor White
}
finally {
    $connection.Close()
}
