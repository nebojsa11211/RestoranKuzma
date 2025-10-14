# Restaurant Suite UI Tests - Playwright for .NET

This project contains end-to-end UI tests for the Restaurant Suite applications using Playwright for .NET with NUnit.

## Project Structure

```
RestaurantSuite.Tests.UI/
├── LoginTests.cs           # Login functionality tests for Guest/Waiter app
├── RestaurantSuite.Tests.UI.csproj
└── bin/Debug/net9.0/
    └── screenshots/        # Test execution screenshots
```

## Prerequisites

- .NET 9.0 SDK
- Playwright browsers (installed automatically)
- Running instances of:
  - Guest/Waiter App: http://localhost:5235
  - API: http://localhost:5213

## Installation

### 1. Install NuGet Packages (Already Done)

The project includes:
- Microsoft.Playwright (1.55.0)
- Microsoft.Playwright.NUnit (1.55.0)
- NUnit (4.2.2)
- Microsoft.NET.Test.Sdk (17.12.0)

### 2. Install Playwright Browsers

```bash
playwright install chromium
```

Or use the PowerShell script:
```powershell
.\bin\Debug\net9.0\playwright.ps1 install
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Login Tests Only
```bash
dotnet test --filter "TestCategory=Login"
```

### Run Smoke Tests
```bash
dotnet test --filter "TestCategory=Smoke"
```

### Run with Verbose Output
```bash
dotnet test --filter "TestCategory=Login" --logger "console;verbosity=detailed"
```

### Run Specific Test
```bash
dotnet test --filter "FullyQualifiedName~LoginWithValidCredentials_ShouldSucceed"
```

## Test Categories

- **Login** - All login-related tests
- **Smoke** - Critical path smoke tests
- **Negative** - Negative test scenarios
- **UI** - UI element validation tests

## Tests Included

### LoginTests.cs

1. **LoginWithValidCredentials_ShouldSucceed**
   - Tests successful login with valid credentials
   - Verifies redirect to home page
   - Confirms authentication state
   - Categories: Login, Smoke

2. **LoginWithInvalidCredentials_ShouldShowError**
   - Tests login with invalid credentials
   - Verifies error message display
   - Confirms user stays on login page
   - Categories: Login, Negative

3. **LoginPage_ShouldDisplayAllRequiredElements**
   - Verifies all UI elements are present
   - Checks form fields, buttons, and links
   - Validates element properties
   - Categories: Login, UI

## Test Account Credentials

**Waiter Account:**
- Email: mark@restaurant.com
- Password: password123
- Role: Waiter

## Screenshots

Screenshots are automatically captured at each test step and saved to:
```
bin/Debug/net9.0/screenshots/{timestamp}/
```

Each test run creates a new timestamped directory with screenshots showing:
- Initial page load
- Form field population
- Button states
- Success/error messages
- Final navigation state

## Example Test Code

```csharp
[Test]
[Category("Login")]
[Category("Smoke")]
public async Task LoginWithValidCredentials_ShouldSucceed()
{
    // Navigate to login page
    await Page.GotoAsync(LoginUrl, new PageGotoOptions
    {
        WaitUntil = WaitUntilState.NetworkIdle
    });

    // Wait for Blazor to initialize
    var emailInput = Page.Locator("#email");
    await emailInput.WaitForAsync(new LocatorWaitForOptions
    {
        State = WaitForSelectorState.Visible,
        Timeout = 30000
    });

    // Fill credentials
    await emailInput.FillAsync(TestEmail);
    await Page.Locator("#password").FillAsync(TestPassword);

    // Submit form
    await Page.Locator("button[type='submit']").ClickAsync();

    // Verify redirect
    await Page.WaitForURLAsync(HomeUrl, new PageWaitForURLOptions
    {
        Timeout = 15000,
        WaitUntil = WaitUntilState.NetworkIdle
    });

    Assert.Pass("Login successful");
}
```

## Key Features

### 1. Blazor Support
- Extended timeouts for Blazor initialization
- Waits for NetworkIdle state
- Handles client-side navigation
- Accounts for component lifecycle

### 2. Robust Element Location
- ID selectors for form fields
- Type selectors for buttons
- CSS class selectors for messages
- Explicit waits for element visibility

### 3. Comprehensive Logging
- Console output at each step
- Detailed verification messages
- Error stack traces
- Test execution timing

### 4. Screenshot Capture
- Automatic screenshots at key steps
- Full-page screenshots
- Timestamped directories
- Error state capture

### 5. Proper Async Patterns
- Async/await throughout
- ConfigureAwait where needed
- Proper timeout handling
- Error handling with try-catch

## Timeouts

Default timeouts are configured for Blazor WebAssembly:
- Default Timeout: 30 seconds
- Navigation Timeout: 30 seconds
- Element Wait Timeout: 30 seconds

These can be adjusted in the SetUp method if needed.

## CI/CD Integration

### GitHub Actions Example

```yaml
name: UI Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'

      - name: Install Playwright
        run: |
          dotnet build tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI
          playwright install chromium

      - name: Start Applications
        run: |
          # Start API and Guest app
          # Add your startup commands here

      - name: Run Tests
        run: dotnet test tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI --logger "console;verbosity=detailed"

      - name: Upload Screenshots
        if: failure()
        uses: actions/upload-artifact@v3
        with:
          name: test-screenshots
          path: tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI/bin/Debug/net9.0/screenshots/
```

## Troubleshooting

### Test Timeout Issues
If tests timeout, ensure:
1. Applications are running (localhost:5235 and localhost:5213)
2. Blazor app has fully initialized
3. Network connection is stable
4. Increase timeout values if needed

### Element Not Found
- Verify element selectors match actual HTML
- Check if element is inside iframe
- Ensure page has fully loaded
- Use `WaitForSelectorAsync` with appropriate timeout

### Screenshots Not Saved
- Check directory permissions
- Verify screenshot directory path
- Ensure disk space is available
- Check console output for errors

### Browser Not Found
Run:
```bash
playwright install chromium
```

## Best Practices

1. **Use Explicit Waits**: Always wait for elements to be visible before interaction
2. **Capture Screenshots**: Take screenshots at key steps for debugging
3. **Descriptive Names**: Use clear, descriptive test and variable names
4. **Assertions**: Include multiple verification points per test
5. **Cleanup**: Ensure proper cleanup in TearDown
6. **Categories**: Tag tests with appropriate categories
7. **Logging**: Add console output for test progress
8. **Error Handling**: Wrap tests in try-catch with screenshot capture

## Future Enhancements

1. Add Page Object Model pattern
2. Implement test data factory
3. Add cross-browser testing (Firefox, WebKit)
4. Create reusable helper methods
5. Add performance testing
6. Implement visual regression testing
7. Add accessibility tests
8. Create test report generation

## Support

For issues or questions:
1. Check test output and screenshots
2. Review Playwright documentation: https://playwright.dev/dotnet/
3. Check NUnit documentation: https://docs.nunit.org/
4. Review test logs for detailed error messages

## License

Same as parent project.
