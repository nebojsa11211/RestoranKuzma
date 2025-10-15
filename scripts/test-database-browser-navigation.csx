#!/usr/bin/env dotnet-script
#r "nuget: Microsoft.Playwright, 1.49.0"

using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

Console.WriteLine("Starting Database Browser Navigation Test...");
Console.WriteLine("==============================================\n");

// Create Playwright instance
using var playwright = await Playwright.CreateAsync();

// Launch browser with headless mode
var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false, // Set to false to see the browser
    SlowMo = 1000 // Slow down by 1 second to see actions
});

// Create browser context with HTTPS errors ignored
var context = await browser.NewContextAsync(new BrowserNewContextOptions
{
    IgnoreHTTPSErrors = true,
    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
});

// Create new page
var page = await context.NewPageAsync();

try
{
    Console.WriteLine("Step 1: Navigate to Admin home page");
    Console.WriteLine("URL: https://localhost:7220");
    await page.GotoAsync("https://localhost:7220", new PageGotoOptions
    {
        WaitUntil = WaitUntilState.NetworkIdle,
        Timeout = 30000
    });

    // Take screenshot of home page
    await page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = @"D:\KimiTest\RestoranKuzma\scripts\screenshots\01-home-page.png",
        FullPage = true
    });
    Console.WriteLine("✓ Screenshot saved: 01-home-page.png");
    Console.WriteLine($"✓ Current URL: {page.Url}");
    Console.WriteLine($"✓ Page Title: {await page.TitleAsync()}");

    // Wait a moment for any dynamic content to load
    await Task.Delay(2000);

    Console.WriteLine("\nStep 2: Locate Database Browser navigation link");

    // Try multiple selectors to find the Database Browser link
    var selectors = new[]
    {
        "a[href='/database-browser']",
        "text=Database Browser",
        "nav >> text=Database Browser",
        ".nav-menu >> text=Database Browser"
    };

    ILocator? databaseBrowserLink = null;
    string? usedSelector = null;

    foreach (var selector in selectors)
    {
        try
        {
            var locator = page.Locator(selector);
            if (await locator.CountAsync() > 0)
            {
                databaseBrowserLink = locator;
                usedSelector = selector;
                Console.WriteLine($"✓ Found link using selector: {selector}");
                break;
            }
        }
        catch
        {
            // Continue to next selector
        }
    }

    if (databaseBrowserLink == null)
    {
        Console.WriteLine("✗ ERROR: Could not find Database Browser navigation link!");
        Console.WriteLine("\nAvailable navigation links:");
        var allLinks = await page.Locator("nav a").AllAsync();
        foreach (var link in allLinks)
        {
            var text = await link.TextContentAsync();
            var href = await link.GetAttributeAsync("href");
            Console.WriteLine($"  - Text: '{text}' | href: '{href}'");
        }
        return;
    }

    // Check if link is visible
    var isVisible = await databaseBrowserLink.IsVisibleAsync();
    Console.WriteLine($"✓ Link is visible: {isVisible}");

    Console.WriteLine("\nStep 3: Click on Database Browser link");

    // Take screenshot before clicking
    await page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = @"D:\KimiTest\RestoranKuzma\scripts\screenshots\02-before-click.png",
        FullPage = true
    });
    Console.WriteLine("✓ Screenshot saved: 02-before-click.png");

    // Click the link
    await databaseBrowserLink.ClickAsync();
    Console.WriteLine("✓ Clicked on Database Browser link");

    // Wait for navigation
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
    {
        Timeout = 10000
    });

    // Wait a moment for Blazor to render
    await Task.Delay(2000);

    Console.WriteLine("\nStep 4: Verify navigation and page content");
    Console.WriteLine($"✓ Current URL: {page.Url}");
    Console.WriteLine($"✓ Page Title: {await page.TitleAsync()}");

    // Check URL
    var expectedUrl = "https://localhost:7220/database-browser";
    var actualUrl = page.Url.TrimEnd('/');
    var urlMatches = actualUrl.Equals(expectedUrl, StringComparison.OrdinalIgnoreCase);
    Console.WriteLine($"✓ URL matches expected: {urlMatches}");
    if (!urlMatches)
    {
        Console.WriteLine($"  Expected: {expectedUrl}");
        Console.WriteLine($"  Actual: {actualUrl}");
    }

    // Take screenshot after navigation
    await page.ScreenshotAsync(new PageScreenshotOptions
    {
        Path = @"D:\KimiTest\RestoranKuzma\scripts\screenshots\03-after-navigation.png",
        FullPage = true
    });
    Console.WriteLine("✓ Screenshot saved: 03-after-navigation.png");

    Console.WriteLine("\nStep 5: Check for page content");

    // Check for Database Browser heading
    var headingLocator = page.Locator("h1, h2, h3").Filter(new LocatorFilterOptions
    {
        HasTextString = "Database Browser"
    });
    var hasHeading = await headingLocator.CountAsync() > 0;
    Console.WriteLine($"✓ Database Browser heading found: {hasHeading}");

    // Check for table list sidebar
    var sidebarSelectors = new[]
    {
        ".database-browser-sidebar",
        ".table-list",
        "aside",
        ".sidebar"
    };

    bool hasSidebar = false;
    foreach (var selector in sidebarSelectors)
    {
        var count = await page.Locator(selector).CountAsync();
        if (count > 0)
        {
            hasSidebar = true;
            Console.WriteLine($"✓ Sidebar found using selector: {selector}");
            break;
        }
    }

    if (!hasSidebar)
    {
        Console.WriteLine("! Warning: Could not find sidebar element");
    }

    // Check for any table names or database content
    var contentIndicators = new[]
    {
        "text=Table",
        "text=Schema",
        "text=Rows",
        ".table-item",
        ".db-table"
    };

    bool hasContent = false;
    foreach (var selector in contentIndicators)
    {
        var count = await page.Locator(selector).CountAsync();
        if (count > 0)
        {
            hasContent = true;
            Console.WriteLine($"✓ Database content found using selector: {selector} (count: {count})");
        }
    }

    if (!hasContent)
    {
        Console.WriteLine("! Warning: Could not find database content indicators");
    }

    // Check for Dashboard content (should NOT be present)
    var dashboardIndicators = new[]
    {
        "text=Dashboard",
        "text=Welcome"
    };

    bool hasDashboardContent = false;
    foreach (var selector in dashboardIndicators)
    {
        var count = await page.Locator(selector).CountAsync();
        if (count > 0)
        {
            hasDashboardContent = true;
            Console.WriteLine($"✗ WARNING: Dashboard content still present! (selector: {selector})");
        }
    }

    if (!hasDashboardContent)
    {
        Console.WriteLine("✓ Dashboard content not present (correct)");
    }

    Console.WriteLine("\nStep 6: Check console for errors");

    // Set up console message listener
    var consoleErrors = new List<string>();
    page.Console += (_, msg) =>
    {
        if (msg.Type == "error")
        {
            consoleErrors.Add(msg.Text);
        }
    };

    // Wait a moment to capture any delayed console messages
    await Task.Delay(2000);

    if (consoleErrors.Count > 0)
    {
        Console.WriteLine($"✗ Console errors found ({consoleErrors.Count}):");
        foreach (var error in consoleErrors)
        {
            Console.WriteLine($"  - {error}");
        }
    }
    else
    {
        Console.WriteLine("✓ No console errors detected");
    }

    Console.WriteLine("\n==============================================");
    Console.WriteLine("Test Summary:");
    Console.WriteLine($"  Navigation successful: {urlMatches}");
    Console.WriteLine($"  Page heading found: {hasHeading}");
    Console.WriteLine($"  Sidebar present: {hasSidebar}");
    Console.WriteLine($"  Database content present: {hasContent}");
    Console.WriteLine($"  Dashboard content absent: {!hasDashboardContent}");
    Console.WriteLine($"  Console errors: {consoleErrors.Count}");
    Console.WriteLine("==============================================");

    if (urlMatches && hasHeading && !hasDashboardContent)
    {
        Console.WriteLine("\n✓✓✓ TEST PASSED ✓✓✓");
    }
    else
    {
        Console.WriteLine("\n✗✗✗ TEST FAILED ✗✗✗");
    }

    // Keep browser open for 5 seconds to allow manual inspection
    Console.WriteLine("\nKeeping browser open for 5 seconds for manual inspection...");
    await Task.Delay(5000);
}
catch (Exception ex)
{
    Console.WriteLine($"\n✗✗✗ TEST ERROR ✗✗✗");
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");

    // Take error screenshot
    try
    {
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = @"D:\KimiTest\RestoranKuzma\scripts\screenshots\error-screenshot.png",
            FullPage = true
        });
        Console.WriteLine("\nError screenshot saved: error-screenshot.png");
    }
    catch { }
}
finally
{
    // Clean up
    await browser.CloseAsync();
    Console.WriteLine("\nBrowser closed.");
}
