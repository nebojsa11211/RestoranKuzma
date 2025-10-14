Add-Type -Path "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\9.0.9\System.Data.dll"

$connectionString = "Data Source=src/RestaurantSuite.Api/restorankuzma.db"

# Load SQLite
Add-Type -AssemblyName System.Data

$connection = New-Object -TypeName Microsoft.Data.Sqlite.SqliteConnection -ArgumentList $connectionString
$connection.Open()

Write-Host "`nDatabase Data Summary:" -ForegroundColor Cyan

$queries = @{
    "Restaurants" = "SELECT COUNT(*) FROM Restaurants"
    "Categories" = "SELECT COUNT(*) FROM Categories"
    "MenuItems" = "SELECT COUNT(*) FROM MenuItems"
    "Users" = "SELECT COUNT(*) FROM Users"
    "Tables" = "SELECT COUNT(*) FROM Tables"
}

foreach ($table in $queries.Keys) {
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = $queries[$table]
    $count = $cmd.ExecuteScalar()
    Write-Host "  $table`: $count"
}

Write-Host "`nRestaurant Details:" -ForegroundColor Cyan
$cmd = $connection.CreateCommand()
$cmd.CommandText = "SELECT Name, Address, Currency FROM Restaurants LIMIT 1"
$reader = $cmd.ExecuteReader()
if ($reader.Read()) {
    Write-Host "  Name: $($reader['Name'])"
    Write-Host "  Address: $($reader['Address'])"
    Write-Host "  Currency: $($reader['Currency'])"
}
$reader.Close()

Write-Host "`nSample User:" -ForegroundColor Cyan
$cmd = $connection.CreateCommand()
$cmd.CommandText = "SELECT Email, FirstName, LastName, Role FROM Users WHERE Role = 'Admin' LIMIT 1"
$reader = $cmd.ExecuteReader()
if ($reader.Read()) {
    Write-Host "  Email: $($reader['Email'])"
    Write-Host "  Name: $($reader['FirstName']) $($reader['LastName'])"
    Write-Host "  Role: $($reader['Role'])"
}
$reader.Close()

$connection.Close()
Write-Host "`nDatabase verification complete!`n" -ForegroundColor Green
