@echo off
echo.
echo ========================================
echo  Killing All Dotnet Processes
echo ========================================
echo.

REM Kill all dotnet.exe processes
echo Stopping all dotnet processes...
taskkill /F /IM dotnet.exe >nul 2>&1

REM Wait a moment
timeout /t 2 /nobreak >nul

REM Check if any dotnet processes are still running
tasklist | find /I "dotnet.exe" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Warning: Some dotnet processes may still be running
    echo Attempting force kill...
    wmic process where "name='dotnet.exe'" delete >nul 2>&1
    timeout /t 1 /nobreak >nul
)

REM Verify ports are free
echo.
echo Checking ports...
netstat -ano | findstr ":5213" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Port 5213 still in use, attempting to free...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5213"') do taskkill /F /PID %%a >nul 2>&1
)

netstat -ano | findstr ":5173" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Port 5173 still in use, attempting to free...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5173"') do taskkill /F /PID %%a >nul 2>&1
)

netstat -ano | findstr ":7046" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Port 7046 still in use, attempting to free...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":7046"') do taskkill /F /PID %%a >nul 2>&1
)

netstat -ano | findstr ":7220" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Port 7220 still in use, attempting to free...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":7220"') do taskkill /F /PID %%a >nul 2>&1
)

echo.
echo ========================================
echo  Status Check
echo ========================================

REM Check if dotnet processes are still running
tasklist | find /I "dotnet.exe" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [X] Some dotnet processes are still running
    echo.
    echo Active dotnet processes:
    tasklist | findstr /I "dotnet.exe"
) else (
    echo [OK] All dotnet processes stopped
)

echo.
echo Port Status:
netstat -ano | findstr ":5213 :5173 :7046 :7220" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Active ports:
    netstat -ano | findstr ":5213 :5173 :7046 :7220"
) else (
    echo [OK] All ports are free (5213, 5173, 7046, 7220)
)

echo.
echo ========================================
echo Done! You can now start your services.
echo ========================================
echo.
pause
