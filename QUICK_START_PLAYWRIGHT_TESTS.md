# Quick Start Guide - Playwright UI Tests

## Summary

Successfully created and executed Playwright tests for the Restaurant Suite Guest/Waiter login functionality.

**Test Results: ALL TESTS PASSED (3/3)**

## What Was Created

### 1. Test Project
- **Location:** `D:\KimiTest\RestoranKuzma\tests\RestaurantSuite.Tests.UI\RestaurantSuite.Tests.UI`
- **Framework:** Playwright for .NET 1.55.0 with NUnit 4.2.2
- **Target:** .NET 9.0

### 2. Test Class
- **File:** `LoginTests.cs`
- **Tests:** 3 comprehensive login tests
- **Features:**
  - Async/await patterns
  - Screenshot capture at each step
  - Comprehensive logging
  - Blazor-specific wait strategies
  - Error handling

### 3. Documentation
- **Test Report:** `PLAYWRIGHT_LOGIN_TEST_REPORT.md` - Detailed test execution report
- **README:** `tests/RestaurantSuite.Tests.UI/README.md` - Project documentation
- **Quick Start:** This file

### 4. Helper Scripts
- **Batch File:** `run-playwright-tests.bat` - Interactive test runner

## Quick Run Commands

### Option 1: Run All Login Tests
```bash
cd "D:\KimiTest\RestoranKuzma\tests\RestaurantSuite.Tests.UI\RestaurantSuite.Tests.UI"
dotnet test --filter "TestCategory=Login"
```

### Option 2: Use Batch File
```bash
cd D:\KimiTest\RestoranKuzma
run-playwright-tests.bat
```

### Option 3: Run from Solution Root
```bash
cd D:\KimiTest\RestoranKuzma
dotnet test tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI.csproj
```

## Test Results Summary

### Test 1: LoginWithValidCredentials_ShouldSucceed
- **Status:** PASSED
- **Time:** ~14 seconds
- **Verified:**
  - Navigation to login page
  - Form field population
  - Button click and submit
  - Redirect to home page
  - Authentication state
  - User "Mark Johnson" displayed

### Test 2: LoginWithInvalidCredentials_ShouldShowError
- **Status:** PASSED
- **Time:** ~2 seconds
- **Verified:**
  - Error message display: "Invalid email or password"
  - User remains on login page
  - No unauthorized access

### Test 3: LoginPage_ShouldDisplayAllRequiredElements
- **Status:** PASSED
- **Time:** ~4 seconds
- **Verified:**
  - Page title
  - Email field
  - Password field (type="password")
  - Login button
  - Remember me checkbox
  - Register link

## Screenshots Location

All screenshots are automatically saved to:
```
tests\RestaurantSuite.Tests.UI\RestaurantSuite.Tests.UI\bin\Debug\net9.0\screenshots\{timestamp}\
```

Latest screenshots show:
1. Login page initial state
2. Email field filled
3. Password field filled
4. Form ready to submit
5. After form submission
6. Home page after successful login
7. Authenticated user state
8. Error message for invalid credentials

## Key Features

### 1. Blazor WebAssembly Support
- Extended timeouts (30 seconds) for Blazor initialization
- Waits for NetworkIdle state
- Handles client-side navigation
- Proper component lifecycle handling

### 2. Robust Element Location
```csharp
// Using ID selectors
var emailInput = Page.Locator("#email");
var passwordInput = Page.Locator("#password");

// Using type selectors
var loginButton = Page.Locator("button[type='submit']");

// Using CSS class selectors
var successMessage = Page.Locator(".alert-success");
var errorMessage = Page.Locator(".alert-error");
```

### 3. Explicit Wait Strategies
```csharp
await emailInput.WaitForAsync(new LocatorWaitForOptions
{
    State = WaitForSelectorState.Visible,
    Timeout = 30000
});
```

### 4. Navigation Handling
```csharp
await Page.WaitForURLAsync(HomeUrl, new PageWaitForURLOptions
{
    Timeout = 15000,
    WaitUntil = WaitUntilState.NetworkIdle
});
```

## Test Account

**Credentials used in tests:**
- Email: mark@restaurant.com
- Password: password123
- Role: Waiter

This account was successfully authenticated and has access to:
- View Menu
- Reserve Table
- Place Order
- Call Service

## Prerequisites

Before running tests, ensure:

1. **Applications are running:**
   - Guest/Waiter App: http://localhost:5235
   - API: http://localhost:5213

2. **Playwright browsers installed:**
   ```bash
   playwright install chromium
   ```

3. **.NET 9.0 SDK installed**

## Viewing Screenshots

### Manual Method
1. Navigate to: `tests\RestaurantSuite.Tests.UI\RestaurantSuite.Tests.UI\bin\Debug\net9.0\screenshots\`
2. Open the most recent timestamp folder
3. View PNG files

### Using Batch File
1. Run `run-playwright-tests.bat`
2. Choose test to run
3. When prompted, press Y to open screenshots folder

## Integration with CI/CD

The tests are ready for CI/CD integration:

```yaml
# Example GitHub Actions workflow
- name: Run Playwright Tests
  run: |
    cd tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI
    dotnet test --logger "console;verbosity=detailed"

- name: Upload Screenshots
  if: failure()
  uses: actions/upload-artifact@v3
  with:
    name: test-screenshots
    path: tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI/bin/Debug/net9.0/screenshots/
```

## Troubleshooting

### Issue: Tests timeout
**Solution:** Ensure both applications are running:
```bash
curl http://localhost:5235
curl http://localhost:5213
```

### Issue: Browser not found
**Solution:** Install Playwright browsers:
```bash
playwright install chromium
```

### Issue: Element not found
**Solution:**
- Check if page has loaded completely
- Verify element selectors in browser DevTools
- Increase timeout values if needed

## Next Steps

### Recommended Enhancements
1. Add tests for other pages (Menu, Orders, Tables)
2. Implement Page Object Model pattern
3. Add cross-browser testing (Firefox, Edge)
4. Create test data factory
5. Add performance tests
6. Implement visual regression testing

### Additional Test Scenarios
1. Test "Remember Me" functionality
2. Test session persistence
3. Test logout functionality
4. Test role-based access control
5. Test password reset flow

## Code Quality

The test code follows best practices:
- Proper async/await usage
- Comprehensive error handling
- Clear test naming
- Detailed logging
- Screenshot documentation
- Multiple assertion points
- Proper test isolation
- Clean setup/teardown

## Performance

Test execution times:
- Single test: ~2-17 seconds
- All login tests: ~23 seconds
- Full test suite: ~23 seconds

Performance is good considering:
- Blazor WebAssembly initialization
- Network requests to API
- Authentication flow
- Page navigation

## Files Created

All files are located in `D:\KimiTest\RestoranKuzma\`:

1. **Test Project:**
   - `tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI/LoginTests.cs`
   - `tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI.csproj`

2. **Documentation:**
   - `PLAYWRIGHT_LOGIN_TEST_REPORT.md` (Detailed report)
   - `tests/RestaurantSuite.Tests.UI/README.md` (Project docs)
   - `QUICK_START_PLAYWRIGHT_TESTS.md` (This file)

3. **Helper Scripts:**
   - `run-playwright-tests.bat` (Interactive test runner)

4. **Screenshots:**
   - `tests/RestaurantSuite.Tests.UI/RestaurantSuite.Tests.UI/bin/Debug/net9.0/screenshots/`

## Summary

The Playwright test suite is fully functional and production-ready. All three login tests pass successfully, demonstrating:
- Successful authentication with valid credentials
- Proper error handling for invalid credentials
- Complete UI element validation

The tests are well-documented, include comprehensive screenshots, and are ready for integration into your CI/CD pipeline.

**Status: COMPLETE - ALL TESTS PASSING**
