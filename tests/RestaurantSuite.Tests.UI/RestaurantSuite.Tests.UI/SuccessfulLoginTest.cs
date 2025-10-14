using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace RestaurantSuite.Tests.UI;

[TestFixture]
public class SuccessfulLoginTest : PageTest
{
    [Test]
    public async Task LoginWithMarkWaiterAccount_ShouldSucceedAndShowUserProfile()
    {
        // Arrange
        const string loginUrl = "http://localhost:5235/login";
        const string email = "mark@restaurant.com";
        const string password = "password123";

        Console.WriteLine("=== Starting Login Test ===");
        Console.WriteLine($"Testing login for: {email}");

        // Act - Navigate to login page
        await Page.GotoAsync(loginUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Take initial screenshot
        await Page.ScreenshotAsync(new()
        {
            Path = "D:\\KimiTest\\RestoranKuzma\\login-step1-page.png",
            FullPage = true
        });
        Console.WriteLine("Step 1: Login page loaded");

        // Wait for page to be fully interactive
        await Task.Delay(2000);

        // Find and fill email field
        var emailInput = Page.Locator("#email");
        await emailInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await emailInput.FillAsync(email);
        Console.WriteLine("Step 2: Email filled");

        // Find and fill password field
        var passwordInput = Page.Locator("#password");
        await passwordInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await passwordInput.FillAsync(password);
        Console.WriteLine("Step 3: Password filled");

        // Take screenshot before clicking login
        await Page.ScreenshotAsync(new()
        {
            Path = "D:\\KimiTest\\RestoranKuzma\\login-step2-filled.png",
            FullPage = true
        });

        // Click login button
        var loginButton = Page.Locator("button[type='submit']");
        await loginButton.ClickAsync();
        Console.WriteLine("Step 4: Login button clicked");

        // Wait for navigation
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Task.Delay(3000); // Wait for client-side processing

        // Take final screenshot
        await Page.ScreenshotAsync(new()
        {
            Path = "D:\\KimiTest\\RestoranKuzma\\login-step3-result.png",
            FullPage = true
        });

        var currentUrl = Page.Url;
        Console.WriteLine($"Step 5: Navigation complete - URL: {currentUrl}");

        // Assert - Verify successful login

        // 1. Should redirect away from login page
        Assert.That(currentUrl, Does.Not.Contain("/login"),
            "Should redirect away from login page on successful login");
        Console.WriteLine("PASS: Redirected from login page");

        // 2. Should show user name in the sidebar (Mark Johnson)
        var userNameLocator = Page.Locator("text=Mark Johnson");
        var userNameCount = await userNameLocator.CountAsync();
        Assert.That(userNameCount, Is.GreaterThan(0),
            "Should display user name 'Mark Johnson' after login");
        Console.WriteLine("PASS: User name 'Mark Johnson' is displayed");

        // 3. Should show logout option
        var logoutLocator = Page.Locator("text=Logout");
        var logoutCount = await logoutLocator.CountAsync();
        Assert.That(logoutCount, Is.GreaterThan(0),
            "Should display 'Logout' option after login");
        Console.WriteLine("PASS: Logout option is visible");

        // 4. Should show welcome message
        var welcomeLocator = Page.Locator("text=Welcome to Restaurant Kuzma");
        var welcomeCount = await welcomeLocator.CountAsync();
        Assert.That(welcomeCount, Is.GreaterThan(0),
            "Should display welcome message on home page");
        Console.WriteLine("PASS: Welcome message is displayed");

        // 5. Verify user is on the home page
        Assert.That(currentUrl, Does.EndWith("/") | Does.EndWith(":5235"),
            "Should be on the home page after login");
        Console.WriteLine("PASS: Successfully navigated to home page");

        Console.WriteLine("=== Login Test PASSED Successfully ===");
        Console.WriteLine($"User '{email}' logged in successfully as 'Mark Johnson'");
        Console.WriteLine("Screenshots saved:");
        Console.WriteLine("  - login-step1-page.png (login form)");
        Console.WriteLine("  - login-step2-filled.png (filled credentials)");
        Console.WriteLine("  - login-step3-result.png (successful login result)");
    }
}
