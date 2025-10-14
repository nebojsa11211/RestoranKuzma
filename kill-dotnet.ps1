# Kill all dotnet processes and free ports
# PowerShell version

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Killing All Dotnet Processes" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Kill all dotnet processes
Write-Host "Stopping all dotnet processes..." -ForegroundColor Yellow
Get-Process -Name dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2

# Check specific ports and kill processes using them
$ports = @(5213, 5173, 7046, 7220, 5054)
Write-Host ""
Write-Host "Freeing up ports..." -ForegroundColor Yellow

foreach ($port in $ports) {
    $connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
    if ($connections) {
        foreach ($conn in $connections) {
            Write-Host "  Killing process on port $port (PID: $($conn.OwningProcess))" -ForegroundColor Gray
            Stop-Process -Id $conn.OwningProcess -Force -ErrorAction SilentlyContinue
        }
    }
}

Start-Sleep -Seconds 1

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Status Check" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Check if dotnet processes are still running
$dotnetProcs = Get-Process -Name dotnet -ErrorAction SilentlyContinue
if ($dotnetProcs) {
    Write-Host "[X] Some dotnet processes are still running:" -ForegroundColor Red
    $dotnetProcs | Format-Table -Property Id, ProcessName, CPU, WorkingSet
} else {
    Write-Host "[OK] All dotnet processes stopped" -ForegroundColor Green
}

# Check port status
Write-Host ""
Write-Host "Port Status:" -ForegroundColor White
$activeConnections = $false
foreach ($port in $ports) {
    $conn = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
    if ($conn) {
        Write-Host "  [X] Port $port is still in use" -ForegroundColor Red
        $activeConnections = $true
    } else {
        Write-Host "  [OK] Port $port is free" -ForegroundColor Green
    }
}

if (-not $activeConnections) {
    Write-Host ""
    Write-Host "[OK] All ports are free!" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Done! You can now start your services." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Keep window open
Read-Host "Press Enter to exit"
