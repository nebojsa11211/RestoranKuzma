using Microsoft.Playwright;
using System.Diagnostics;

namespace NavLinkTest;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("=== Blazor Navigation Link Test ===\n");

        // Start the Blazor application
        Console.WriteLine("Starting Blazor Server application...");
        var blazorProcess = StartBlazorApp();

        if (blazorProcess == null)
        {
            Console.WriteLine("ERROR: Failed to start Blazor application");
            return 1;
        }

        try
        {
            // Wait for application to start
            Console.WriteLine("Waiting for application to start...");
            await Task.Delay(15000); // Increased wait time

            // Run Playwright test
            using var playwright = await Playwright.CreateAsync();
            await RunNavigationTest(playwright);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return 1;
        }
        finally
        {
            // Stop the Blazor application
            Console.WriteLine("\nStopping Blazor application...");
            if (blazorProcess != null && !blazorProcess.HasExited)
            {
                blazorProcess.Kill(true);
                blazorProcess.WaitForExit();
            }
            Console.WriteLine("Test completed.");
        }
    }

    static Process? StartBlazorApp()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project \"D:\\KimiTest\\RestoranKuzma\\src\\RestaurantSuite.Admin\\RestaurantSuite.Admin.csproj\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);

            if (process != null)
            {
                // Capture output for debugging
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine($"[APP] {e.Data}");
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine($"[APP ERROR] {e.Data}");
                    }
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            return process;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start Blazor app: {ex.Message}");
            return null;
        }
    }

    static async Task RunNavigationTest(IPlaywright playwright)
    {
        Console.WriteLine("\n=== Starting Playwright Browser Test ===\n");

        // Launch browser in headed mode so we can see what's happening
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false, // Run in visible mode
            SlowMo = 1000 // Slow down operations by 1 second for visibility
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        // Enable console logging
        var page = await context.NewPageAsync();
        page.Console += (_, msg) =>
        {
            Console.WriteLine($"[BROWSER CONSOLE - {msg.Type}] {msg.Text}");
        };

        page.PageError += (_, error) =>
        {
            Console.WriteLine($"[BROWSER ERROR] {error}");
        };

        try
        {
            // Navigate to the application
            Console.WriteLine("Navigating to https://localhost:7220...");
            var response = await page.GotoAsync("https://localhost:7220", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 60000
            });

            Console.WriteLine($"Page loaded with status: {response?.Status}");

            // Take initial screenshot
            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = "D:\\KimiTest\\RestoranKuzma\\tests\\NavLinkTest\\screenshot-initial.png",
                FullPage = true
            });
            Console.WriteLine("Screenshot saved: screenshot-initial.png");

            // Wait for Blazor to initialize
            Console.WriteLine("\nWaiting for Blazor to initialize...");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(2000);

            // Check if we need to authenticate
            var currentUrl = page.Url;
            Console.WriteLine($"Current URL: {currentUrl}");

            if (currentUrl.Contains("/Identity/Account/Login") || currentUrl.Contains("login"))
            {
                Console.WriteLine("\n=== Authentication Required ===");
                Console.WriteLine("Attempting to login...");

                await page.FillAsync("input[name='Input.Email']", "admin@restaurantsuite.com");
                await page.FillAsync("input[name='Input.Password']", "Admin123!");

                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = "D:\\KimiTest\\RestoranKuzma\\tests\\NavLinkTest\\screenshot-login-form.png"
                });

                await page.ClickAsync("button[type='submit']");
                Console.WriteLine("Login form submitted");

                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Task.Delay(2000);

                Console.WriteLine($"After login URL: {page.Url}");
            }

            // Now test the navigation link
            Console.WriteLine("\n=== Testing Navigation Link ===");

            // Check if the navigation menu is visible
            var navMenuVisible = await page.Locator("#nav-menu").IsVisibleAsync();
            Console.WriteLine($"Navigation menu visible: {navMenuVisible}");

            // Look for the Database Browser link
            Console.WriteLine("\nLooking for Database Browser link...");

            // Try multiple strategies to locate the link
            var linkLocator = page.Locator("a[href='database-browser']");
            var linkCount = await linkLocator.CountAsync();
            Console.WriteLine($"Found {linkCount} link(s) with href='database-browser'");

            if (linkCount > 0)
            {
                var linkVisible = await linkLocator.First.IsVisibleAsync();
                Console.WriteLine($"Link visible: {linkVisible}");

                if (linkVisible)
                {
                    // Take screenshot before clicking
                    await page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = "D:\\KimiTest\\RestoranKuzma\\tests\\NavLinkTest\\screenshot-before-click.png",
                        FullPage = true
                    });

                    Console.WriteLine("\nAttempting to click the Database Browser link...");

                    // Scroll to the link if needed
                    await linkLocator.First.ScrollIntoViewIfNeededAsync();
                    await Task.Delay(500);

                    // Highlight the link
                    await page.EvaluateAsync(@"
                        (selector) => {
                            const element = document.querySelector(selector);
                            if (element) {
                                element.style.border = '3px solid red';
                                element.style.backgroundColor = 'yellow';
                            }
                        }", "a[href='database-browser']");

                    await Task.Delay(1000);

                    // Click the link
                    await linkLocator.First.ClickAsync();
                    Console.WriteLine("Link clicked!");

                    // Wait for navigation
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await Task.Delay(2000);

                    // Check the new URL
                    var newUrl = page.Url;
                    Console.WriteLine($"\nAfter click URL: {newUrl}");

                    // Take screenshot after navigation
                    await page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = "D:\\KimiTest\\RestoranKuzma\\tests\\NavLinkTest\\screenshot-after-click.png",
                        FullPage = true
                    });

                    // Wait a bit longer for Blazor to render the new page
                    await Task.Delay(3000);

                    // Check for Database Browser page content
                    var pageTitleLocator = page.Locator("h1.db-title");
                    try
                    {
                        await pageTitleLocator.WaitForAsync(new LocatorWaitForOptions
                        {
                            State = WaitForSelectorState.Visible,
                            Timeout = 10000
                        });

                        var pageTitle = await pageTitleLocator.TextContentAsync();
                        Console.WriteLine($"\n✓ SUCCESS: Navigated to Database Browser page");
                        Console.WriteLine($"  Page title: {pageTitle}");

                        // Check if the page components loaded
                        var sidebarLocator = page.Locator(".db-sidebar");
                        var sidebarVisible = await sidebarLocator.IsVisibleAsync();
                        Console.WriteLine($"  Sidebar visible: {sidebarVisible}");

                        var contentLocator = page.Locator(".db-content");
                        var contentVisible = await contentLocator.IsVisibleAsync();
                        Console.WriteLine($"  Content area visible: {contentVisible}");
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine($"\n✗ ERROR: URL changed but page content failed to load");
                        Console.WriteLine("  The navigation worked but the Database Browser page didn't render");
                        Console.WriteLine("  This indicates a problem with the page component or API calls");
                    }

                    // Check for any error messages
                    var errorLocator = page.Locator(".db-error-state, .error, .alert-danger");
                    var errorCount = await errorLocator.CountAsync();
                    if (errorCount > 0)
                    {
                        Console.WriteLine($"\n✗ ERROR: Found {errorCount} error message(s) on page");
                        for (int i = 0; i < errorCount; i++)
                        {
                            var errorText = await errorLocator.Nth(i).TextContentAsync();
                            Console.WriteLine($"  Error {i + 1}: {errorText}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\n✗ ERROR: Link found but not visible");
                    Console.WriteLine("  The link exists in the DOM but is not visible to users");
                }
            }
            else
            {
                Console.WriteLine("\n✗ ERROR: Database Browser link not found");
                Console.WriteLine("  The link with href='database-browser' does not exist in the DOM");

                // List all navigation links for debugging
                var allLinks = await page.Locator("#nav-menu a").AllAsync();
                Console.WriteLine($"\nFound {allLinks.Count} navigation links:");
                foreach (var link in allLinks)
                {
                    var href = await link.GetAttributeAsync("href");
                    var text = await link.TextContentAsync();
                    Console.WriteLine($"  - {text?.Trim()} -> {href}");
                }
            }

            // Keep browser open for a few seconds to observe
            Console.WriteLine("\nKeeping browser open for 5 seconds...");
            await Task.Delay(5000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ TEST FAILED: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            // Take error screenshot
            try
            {
                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = "D:\\KimiTest\\RestoranKuzma\\tests\\NavLinkTest\\screenshot-error.png",
                    FullPage = true
                });
                Console.WriteLine("Error screenshot saved: screenshot-error.png");
            }
            catch { }
        }
        finally
        {
            await browser.CloseAsync();
        }
    }
}
