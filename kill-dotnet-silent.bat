@echo off
REM Silent version - kills dotnet processes without output

taskkill /F /IM dotnet.exe >nul 2>&1
timeout /t 1 /nobreak >nul

REM Force kill any remaining processes on specific ports
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5213" 2^>nul') do taskkill /F /PID %%a >nul 2>&1
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5173" 2^>nul') do taskkill /F /PID %%a >nul 2>&1
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":7046" 2^>nul') do taskkill /F /PID %%a >nul 2>&1
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":7220" 2^>nul') do taskkill /F /PID %%a >nul 2>&1

echo Dotnet processes killed and ports freed.
