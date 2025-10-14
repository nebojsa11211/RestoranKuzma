@echo off
echo Starting Restaurant Suite Development Environment...
echo.
echo This will start:
echo   - API on http://localhost:5213
echo   - Chef App on https://localhost:7046
echo.
pause

REM Start API in new window
start "Restaurant API" cmd /k "cd src\RestaurantSuite.Api && dotnet run"

REM Wait for API to start
timeout /t 5 /nobreak > nul

REM Start Chef with watch in new window
start "Chef App" cmd /k "cd src\RestaurantSuite.Chef && dotnet watch run"

echo.
echo ✅ Services starting in separate windows
echo.
echo URLs:
echo   API: http://localhost:5213
echo   Chef: https://localhost:7046
echo.
echo Close the command windows to stop the services.
pause
