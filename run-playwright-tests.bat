@echo off
REM Playwright UI Tests Runner for Restaurant Suite
REM This script runs the Playwright tests for the Guest/Waiter application

echo ========================================
echo Restaurant Suite - Playwright UI Tests
echo ========================================
echo.

REM Check if applications are running
echo Checking if applications are running...
curl -s -o nul -w "Guest App (localhost:5235): %%{http_code}\n" http://localhost:5235
curl -s -o nul -w "API (localhost:5213): %%{http_code}\n" http://localhost:5213
echo.

REM Navigate to test project
cd /d "%~dp0tests\RestaurantSuite.Tests.UI\RestaurantSuite.Tests.UI"

echo Select test suite to run:
echo.
echo 1. All Login Tests
echo 2. Smoke Tests Only
echo 3. Valid Login Test
echo 4. Invalid Login Test
echo 5. UI Elements Test
echo 6. All Tests
echo.

set /p choice="Enter your choice (1-6): "

if "%choice%"=="1" (
    echo Running all login tests...
    dotnet test --filter "TestCategory=Login" --logger "console;verbosity=detailed"
) else if "%choice%"=="2" (
    echo Running smoke tests...
    dotnet test --filter "TestCategory=Smoke" --logger "console;verbosity=detailed"
) else if "%choice%"=="3" (
    echo Running valid login test...
    dotnet test --filter "FullyQualifiedName~LoginWithValidCredentials_ShouldSucceed" --logger "console;verbosity=detailed"
) else if "%choice%"=="4" (
    echo Running invalid login test...
    dotnet test --filter "FullyQualifiedName~LoginWithInvalidCredentials_ShouldShowError" --logger "console;verbosity=detailed"
) else if "%choice%"=="5" (
    echo Running UI elements test...
    dotnet test --filter "FullyQualifiedName~LoginPage_ShouldDisplayAllRequiredElements" --logger "console;verbosity=detailed"
) else if "%choice%"=="6" (
    echo Running all tests...
    dotnet test --logger "console;verbosity=detailed"
) else (
    echo Invalid choice. Running all login tests by default...
    dotnet test --filter "TestCategory=Login" --logger "console;verbosity=detailed"
)

echo.
echo ========================================
echo Test execution completed!
echo.
echo Screenshots saved to:
echo %CD%\bin\Debug\net9.0\screenshots\
echo.

REM Open screenshots folder
set /p open="Open screenshots folder? (Y/N): "
if /i "%open%"=="Y" (
    explorer "bin\Debug\net9.0\screenshots"
)

pause
