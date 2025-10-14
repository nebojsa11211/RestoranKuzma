# Restaurant Suite Development Startup Script
# This script starts both API and Chef app for development

Write-Host "🍽️  Restaurant Suite - Development Startup" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Check if ports are available
Write-Host "Checking ports..." -ForegroundColor Yellow

function Test-Port {
    param($Port)
    $connection = Test-NetConnection -ComputerName localhost -Port $Port -WarningAction SilentlyContinue -InformationLevel Quiet
    return $connection
}

# Kill existing dotnet processes if needed
$apiInUse = Test-Port -Port 5213
$chefInUse = Test-Port -Port 7046

if ($apiInUse -or $chefInUse) {
    Write-Host "⚠️  Ports already in use. Cleaning up..." -ForegroundColor Yellow
    Get-Process -Name dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Seconds 2
    Write-Host "✅ Cleaned up old processes" -ForegroundColor Green
}

Write-Host ""
Write-Host "Starting services..." -ForegroundColor Yellow
Write-Host ""

# Start API in background
Write-Host "🚀 Starting API on http://localhost:5213" -ForegroundColor Green
$apiJob = Start-Job -ScriptBlock {
    Set-Location "D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Api"
    dotnet run --no-build
}

# Wait a bit for API to start
Start-Sleep -Seconds 3

# Start Chef with watch
Write-Host "🚀 Starting Chef App on https://localhost:7046" -ForegroundColor Green
Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Both services are starting..." -ForegroundColor Cyan
Write-Host ""
Write-Host "📝 URLs:" -ForegroundColor White
Write-Host "   API:     http://localhost:5213" -ForegroundColor White
Write-Host "   Swagger: http://localhost:5213/swagger" -ForegroundColor White
Write-Host "   Chef:    https://localhost:7046" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop both services" -ForegroundColor Yellow
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

try {
    # Run Chef in foreground with watch for hot reload
    Set-Location "D:\KimiTest\RestoranKuzma\src\RestaurantSuite.Chef"
    dotnet watch run
}
finally {
    # Cleanup when script ends
    Write-Host ""
    Write-Host "Stopping services..." -ForegroundColor Yellow
    Stop-Job -Job $apiJob -ErrorAction SilentlyContinue
    Remove-Job -Job $apiJob -ErrorAction SilentlyContinue
    Get-Process -Name dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "✅ All services stopped" -ForegroundColor Green
}
