using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace RestaurantSuite.Tests.UI;

[TestFixture]
public class QuickLoginTest : PageTest
{
    [Test]
    public async Task LoginWithMarkWaiterAccount_ShouldSucceed()
    {
        // Arrange
        const string loginUrl = "http://localhost:5235/login";
        const string email = "mark@restaurant.com";
        const string password = "password123";

        // Act - Navigate to login page
        await Page.GotoAsync(loginUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Take screenshot of login page to see structure
        await Page.ScreenshotAsync(new()
        {
            Path = "D:\\KimiTest\\RestoranKuzma\\tests\\RestaurantSuite.Tests.UI\\RestaurantSuite.Tests.UI\\login-page.png",
            FullPage = true
        });

        Console.WriteLine("Login page screenshot saved");
        Console.WriteLine($"Page URL: {Page.Url}");
        Console.WriteLine($"Page Title: {await Page.TitleAsync()}");

        // Get page content to inspect
        var pageContent = await Page.ContentAsync();
        await File.WriteAllTextAsync("D:\\KimiTest\\RestoranKuzma\\tests\\RestaurantSuite.Tests.UI\\RestaurantSuite.Tests.UI\\login-page.html", pageContent);
        Console.WriteLine("Page HTML saved to login-page.html");

        // Try to find input fields with more flexible selectors
        var inputs = await Page.Locator("input").AllAsync();
        Console.WriteLine($"Found {inputs.Count} input elements");

        for (int i = 0; i < inputs.Count; i++)
        {
            var type = await inputs[i].GetAttributeAsync("type") ?? "unknown";
            var name = await inputs[i].GetAttributeAsync("name") ?? "unknown";
            var id = await inputs[i].GetAttributeAsync("id") ?? "unknown";
            var placeholder = await inputs[i].GetAttributeAsync("placeholder") ?? "unknown";
            Console.WriteLine($"Input {i}: type={type}, name={name}, id={id}, placeholder={placeholder}");
        }

        // Wait for the page to be fully interactive
        await Task.Delay(3000);

        // Try different selectors for email field
        ILocator? emailInput = null;
        var selectors = new[]
        {
            "input[type='email']",
            "input[name='email']",
            "input[placeholder*='email' i]",
            "#email",
            "[id*='email' i]",
            "input[type='text']" // Sometimes email is text type
        };

        foreach (var selector in selectors)
        {
            try
            {
                var locator = Page.Locator(selector).First;
                if (await locator.CountAsync() > 0)
                {
                    emailInput = locator;
                    Console.WriteLine($"Found email input with selector: {selector}");
                    break;
                }
            }
            catch { }
        }

        Assert.That(emailInput, Is.Not.Null, "Could not find email input field");

        // Try different selectors for password field
        ILocator? passwordInput = null;
        var passwordSelectors = new[]
        {
            "input[type='password']",
            "input[name='password']",
            "input[placeholder*='password' i]",
            "#password",
            "[id*='password' i]"
        };

        foreach (var selector in passwordSelectors)
        {
            try
            {
                var locator = Page.Locator(selector).First;
                if (await locator.CountAsync() > 0)
                {
                    passwordInput = locator;
                    Console.WriteLine($"Found password input with selector: {selector}");
                    break;
                }
            }
            catch { }
        }

        Assert.That(passwordInput, Is.Not.Null, "Could not find password input field");

        // Fill in credentials
        await emailInput!.FillAsync(email);
        await passwordInput!.FillAsync(password);

        Console.WriteLine("Filled in credentials");

        // Find and click login button
        var buttonSelectors = new[]
        {
            "button[type='submit']",
            "button:has-text('Login')",
            "button:has-text('Sign in')",
            "input[type='submit']",
            "button"
        };

        ILocator? loginButton = null;
        foreach (var selector in buttonSelectors)
        {
            try
            {
                var locator = Page.Locator(selector).First;
                if (await locator.CountAsync() > 0)
                {
                    loginButton = locator;
                    Console.WriteLine($"Found login button with selector: {selector}");
                    break;
                }
            }
            catch { }
        }

        Assert.That(loginButton, Is.Not.Null, "Could not find login button");

        await loginButton!.ClickAsync();
        Console.WriteLine("Clicked login button");

        // Wait for navigation after login
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Task.Delay(3000); // Additional wait for any client-side redirects

        // Take screenshot of result
        await Page.ScreenshotAsync(new()
        {
            Path = "D:\\KimiTest\\RestoranKuzma\\tests\\RestaurantSuite.Tests.UI\\RestaurantSuite.Tests.UI\\login-result.png",
            FullPage = true
        });

        // Assert - Verify successful login
        var currentUrl = Page.Url;
        Console.WriteLine($"Current URL after login: {currentUrl}");

        // Check we're not on the login page anymore (successful redirect)
        var isStillOnLogin = currentUrl.Contains("/login");

        if (isStillOnLogin)
        {
            // Check if there's an error message
            var pageText = await Page.TextContentAsync("body");
            Console.WriteLine($"Still on login page. Page text: {pageText}");
        }

        Assert.That(currentUrl, Does.Not.Contain("/login"),
            "Should redirect away from login page on success");

        // Check for common error indicators
        var errorLocators = await Page.Locator("text=/error|invalid|failed/i").AllAsync();
        if (errorLocators.Any())
        {
            foreach (var errorLoc in errorLocators)
            {
                var errorText = await errorLoc.TextContentAsync();
                Console.WriteLine($"Found error text: {errorText}");
            }
        }

        Assert.That(errorLocators, Is.Empty,
            "Should not show any error messages");

        Console.WriteLine("Login test completed successfully!");
        Console.WriteLine($"Screenshots saved to login-page.png and login-result.png");
    }
}
