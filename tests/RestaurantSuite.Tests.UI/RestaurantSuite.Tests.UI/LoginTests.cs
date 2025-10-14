using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace RestaurantSuite.Tests.UI;

/// <summary>
/// Tests for the Guest/Waiter application login functionality
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoginTests : PageTest
{
    // Configuration
    private const string BaseUrl = "http://localhost:5235";
    private const string LoginUrl = $"{BaseUrl}/login";
    private const string HomeUrl = $"{BaseUrl}/";

    // Test credentials
    private const string TestEmail = "mark@restaurant.com";
    private const string TestPassword = "password123";
    private const string TestRole = "Waiter";

    // Screenshot directory
    private string _screenshotDirectory = string.Empty;

    [SetUp]
    public void SetUpTest()
    {
        // Create screenshots directory
        _screenshotDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "screenshots",
            DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        );
        Directory.CreateDirectory(_screenshotDirectory);

        // Set default timeout
        Page.SetDefaultTimeout(30000); // 30 seconds
        Page.SetDefaultNavigationTimeout(30000);

        Console.WriteLine($"Starting test: {TestContext.CurrentContext.Test.Name}");
        Console.WriteLine($"Screenshots will be saved to: {_screenshotDirectory}");
    }

    [Test]
    [Category("Login")]
    [Category("Smoke")]
    public async Task LoginWithValidCredentials_ShouldSucceed()
    {
        try
        {
            // Step 1: Navigate to login page
            Console.WriteLine($"Step 1: Navigating to login page: {LoginUrl}");
            await Page.GotoAsync(LoginUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Take screenshot of login page
            await TakeScreenshotAsync("01-login-page-loaded");

            // Verify we're on the login page
            await Expect(Page).ToHaveURLAsync(LoginUrl);
            Console.WriteLine("Verified: Successfully navigated to login page");

            // Wait for Blazor to fully initialize by checking for the login form
            var emailInput = Page.Locator("#email");
            await emailInput.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 30000
            });
            Console.WriteLine("Verified: Login form is visible and ready");

            // Step 2: Fill in email field
            Console.WriteLine($"Step 2: Filling in email field with: {TestEmail}");
            await emailInput.FillAsync(TestEmail);
            await TakeScreenshotAsync("02-email-filled");

            // Verify email was entered
            var emailValue = await emailInput.InputValueAsync();
            Assert.That(emailValue, Is.EqualTo(TestEmail),
                "Email field should contain the test email");
            Console.WriteLine("Verified: Email field contains correct value");

            // Step 3: Fill in password field
            Console.WriteLine("Step 3: Filling in password field");
            var passwordInput = Page.Locator("#password");
            await passwordInput.FillAsync(TestPassword);
            await TakeScreenshotAsync("03-password-filled");

            // Verify password was entered (we can check if it's not empty)
            var passwordValue = await passwordInput.InputValueAsync();
            Assert.That(passwordValue, Is.Not.Empty,
                "Password field should not be empty");
            Console.WriteLine("Verified: Password field is filled");

            // Step 4: Click the login button
            Console.WriteLine("Step 4: Clicking login button");
            var loginButton = Page.Locator("button[type='submit']");

            // Verify button is enabled before clicking
            await Expect(loginButton).ToBeEnabledAsync();

            await TakeScreenshotAsync("04-before-submit");

            // Click and wait for navigation
            await loginButton.ClickAsync();
            Console.WriteLine("Login button clicked, waiting for response...");

            // Wait a moment for the form to submit
            await Page.WaitForTimeoutAsync(1000);
            await TakeScreenshotAsync("05-after-submit");

            // Step 5: Wait for success message or navigation
            Console.WriteLine("Step 5: Waiting for success message or redirect");

            // Option A: Check for success message
            var successMessage = Page.Locator(".alert-success");
            try
            {
                await successMessage.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 10000
                });
                var messageText = await successMessage.TextContentAsync();
                Console.WriteLine($"Success message displayed: {messageText}");
                await TakeScreenshotAsync("06-success-message");

                Assert.That(messageText, Does.Contain("successful").IgnoreCase,
                    "Success message should indicate successful login");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Success message not found, checking for navigation...");
            }

            // Step 6: Verify redirect to home page
            Console.WriteLine("Step 6: Verifying redirect to home page");

            // Wait for navigation to complete (with extended timeout for Blazor)
            try
            {
                await Page.WaitForURLAsync(HomeUrl, new PageWaitForURLOptions
                {
                    Timeout = 15000,
                    WaitUntil = WaitUntilState.NetworkIdle
                });
                Console.WriteLine($"Verified: Redirected to home page: {HomeUrl}");
            }
            catch (TimeoutException)
            {
                var currentUrl = Page.Url;
                Console.WriteLine($"Warning: Expected redirect to {HomeUrl}, but current URL is: {currentUrl}");

                // If we're on the home page (even without trailing slash), consider it success
                if (currentUrl.TrimEnd('/') == BaseUrl || currentUrl == HomeUrl)
                {
                    Console.WriteLine("Verification: On home page (with or without trailing slash)");
                }
                else
                {
                    await TakeScreenshotAsync("07-unexpected-url");
                    Assert.Fail($"Expected to be redirected to {HomeUrl}, but URL is: {currentUrl}");
                }
            }

            await TakeScreenshotAsync("07-home-page");

            // Step 7: Verify authentication state
            Console.WriteLine("Step 7: Verifying authentication state");

            // Check for elements that should only appear when authenticated
            // This depends on your app's structure, but common checks include:

            // Check if we're NOT on the login page anymore
            var currentUrlFinal = Page.Url;
            Assert.That(currentUrlFinal, Does.Not.Contain("/login"),
                "Should not be on login page after successful login");

            // Check for authenticated user elements (adjust based on your app)
            // For example, check if there's a logout button or user menu
            try
            {
                // Wait for app to fully render
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Check if we can access the main content area (not just login form)
                var mainContent = Page.Locator(".main-content, main, [role='main']");
                if (await mainContent.CountAsync() > 0)
                {
                    Console.WriteLine("Verified: Main content area is visible (user is authenticated)");
                }

                await TakeScreenshotAsync("08-authenticated-state");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: Could not verify additional authenticated state: {ex.Message}");
            }

            // Final verification
            Console.WriteLine("Test completed successfully!");
            Assert.Pass("Login test completed successfully. User authenticated and redirected to home page.");
        }
        catch (Exception ex)
        {
            await TakeScreenshotAsync("error-state");
            Console.WriteLine($"Test failed with error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    [Test]
    [Category("Login")]
    [Category("Negative")]
    public async Task LoginWithInvalidCredentials_ShouldShowError()
    {
        try
        {
            // Navigate to login page
            Console.WriteLine($"Navigating to login page: {LoginUrl}");
            await Page.GotoAsync(LoginUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            await TakeScreenshotAsync("01-login-page");

            // Wait for form to be ready
            var emailInput = Page.Locator("#email");
            await emailInput.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible
            });

            // Fill in invalid credentials
            Console.WriteLine("Filling in invalid credentials");
            await emailInput.FillAsync("invalid@email.com");
            await Page.Locator("#password").FillAsync("wrongpassword");

            await TakeScreenshotAsync("02-invalid-credentials-filled");

            // Click login button
            Console.WriteLine("Clicking login button");
            await Page.Locator("button[type='submit']").ClickAsync();

            // Wait for error message
            Console.WriteLine("Waiting for error message");
            var errorMessage = Page.Locator(".alert-error");
            await errorMessage.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            await TakeScreenshotAsync("03-error-message-displayed");

            // Verify error message is displayed
            var errorText = await errorMessage.TextContentAsync();
            Console.WriteLine($"Error message displayed: {errorText}");

            Assert.That(errorText, Is.Not.Null.And.Not.Empty,
                "Error message should be displayed");

            // Verify we're still on login page
            await Expect(Page).ToHaveURLAsync(new Regex(".*login.*"));
            Console.WriteLine("Verified: Still on login page after failed login");

            Assert.Pass("Invalid login correctly shows error message");
        }
        catch (Exception ex)
        {
            await TakeScreenshotAsync("error-state");
            Console.WriteLine($"Test failed with error: {ex.Message}");
            throw;
        }
    }

    [Test]
    [Category("Login")]
    [Category("UI")]
    public async Task LoginPage_ShouldDisplayAllRequiredElements()
    {
        try
        {
            // Navigate to login page
            Console.WriteLine($"Navigating to login page: {LoginUrl}");
            await Page.GotoAsync(LoginUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            await TakeScreenshotAsync("login-page-elements");

            // Verify page title
            await Expect(Page).ToHaveTitleAsync(new Regex(".*Login.*", RegexOptions.IgnoreCase));
            Console.WriteLine("Verified: Page title contains 'Login'");

            // Verify email field
            var emailInput = Page.Locator("#email");
            await Expect(emailInput).ToBeVisibleAsync();
            Console.WriteLine("Verified: Email input field is visible");

            // Verify password field
            var passwordInput = Page.Locator("#password");
            await Expect(passwordInput).ToBeVisibleAsync();
            await Expect(passwordInput).ToHaveAttributeAsync("type", "password");
            Console.WriteLine("Verified: Password input field is visible with type='password'");

            // Verify login button
            var loginButton = Page.Locator("button[type='submit']");
            await Expect(loginButton).ToBeVisibleAsync();
            await Expect(loginButton).ToBeEnabledAsync();
            Console.WriteLine("Verified: Login button is visible and enabled");

            // Verify remember me checkbox
            var rememberMeCheckbox = Page.Locator("input[type='checkbox']");
            await Expect(rememberMeCheckbox).ToBeVisibleAsync();
            Console.WriteLine("Verified: Remember me checkbox is visible");

            // Verify register link
            var registerLink = Page.Locator("a[href='/register']");
            await Expect(registerLink).ToBeVisibleAsync();
            Console.WriteLine("Verified: Register link is visible");

            Assert.Pass("All required UI elements are present on login page");
        }
        catch (Exception ex)
        {
            await TakeScreenshotAsync("error-state");
            Console.WriteLine($"Test failed with error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Helper method to take screenshots with consistent naming
    /// </summary>
    private async Task TakeScreenshotAsync(string stepName)
    {
        try
        {
            var screenshotPath = Path.Combine(
                _screenshotDirectory,
                $"{stepName}.png"
            );

            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });

            Console.WriteLine($"Screenshot saved: {screenshotPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to take screenshot '{stepName}': {ex.Message}");
        }
    }

    [TearDown]
    public async Task TearDownTest()
    {
        var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
        Console.WriteLine($"Test {TestContext.CurrentContext.Test.Name} completed with status: {testStatus}");

        if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            await TakeScreenshotAsync("final-error-state");
        }
    }
}
