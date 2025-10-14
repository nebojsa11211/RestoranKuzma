using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        using var playwright = await Playwright.CreateAsync();

        // Launch browser
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
        });

        var page = await context.NewPageAsync();

        Console.WriteLine("Navigating to https://localhost:7220...");

        try
        {
            // Navigate to the Admin application
            await page.GotoAsync("https://localhost:7220", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            Console.WriteLine("Page loaded successfully.");

            // Wait for Blazor to render - wait for any navigation element
            await page.WaitForSelectorAsync("nav, .sidebar, [role='navigation']", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 15000
            });

            Console.WriteLine("Navigation element is visible.");

            // Take a screenshot of the entire page
            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = @"D:\KimiTest\RestoranKuzma\playwright_test\admin_nav_full.png",
                FullPage = true
            });

            Console.WriteLine("Full page screenshot saved.");

            // Get the page HTML to analyze structure
            var pageHtml = await page.ContentAsync();

            // Check for the styled nav-menu element
            var styledNavMenu = await page.QuerySelectorAsync("#nav-menu");
            Console.WriteLine($"\n=== STYLED NAV MENU CHECK ===");
            Console.WriteLine($"Styled nav menu (#nav-menu) found: {styledNavMenu != null}");

            // Check for nav-icon-recipes
            var recipesIcon = await page.QuerySelectorAsync("#nav-icon-recipes");
            Console.WriteLine($"Recipes icon (#nav-icon-recipes) found: {recipesIcon != null}");

            // Get all navigation links using various selectors
            Console.WriteLine("\n=== FINDING ALL NAVIGATION LINKS ===");

            // Method 1: Try to find links with href containing text
            var allLinks = await page.QuerySelectorAllAsync("a");
            Console.WriteLine($"Total <a> tags found: {allLinks.Count}");

            var navTexts = new List<string>();
            foreach (var link in allLinks)
            {
                var href = await link.GetAttributeAsync("href");
                var text = await link.TextContentAsync();
                var isVisible = await link.IsVisibleAsync();

                if (isVisible && !string.IsNullOrWhiteSpace(text))
                {
                    var trimmedText = text.Trim();
                    Console.WriteLine($"Link: '{trimmedText}' | Href: '{href}' | Visible: {isVisible}");
                    navTexts.Add(trimmedText);
                }
            }

            // Check specifically for Recipes
            Console.WriteLine("\n=== RECIPES LINK VERIFICATION ===");
            var recipesLinkByHref = await page.QuerySelectorAsync("a[href='recipes'], a[href='/recipes']");
            Console.WriteLine($"Recipes link by href: {recipesLinkByHref != null}");

            var recipesLinkByText = await page.QuerySelectorAsync("text='Recipes'");
            Console.WriteLine($"Recipes link by text: {recipesLinkByText != null}");

            // Try to find the actual navigation structure
            Console.WriteLine("\n=== NAVIGATION STRUCTURE ===");
            var navElements = await page.QuerySelectorAllAsync("nav");
            Console.WriteLine($"Found {navElements.Count} <nav> elements");

            for (int i = 0; i < navElements.Count; i++)
            {
                var nav = navElements[i];
                var navId = await nav.GetAttributeAsync("id");
                var navClass = await nav.GetAttributeAsync("class");
                var isVisible = await nav.IsVisibleAsync();
                Console.WriteLine($"Nav {i + 1}: id='{navId}' class='{navClass}' visible={isVisible}");

                // Get innerHTML to see what's inside
                var innerHTML = await nav.InnerHTMLAsync();
                if (innerHTML.Length > 500)
                {
                    Console.WriteLine($"  Content (first 500 chars): {innerHTML.Substring(0, 500)}...");
                }
                else
                {
                    Console.WriteLine($"  Content: {innerHTML}");
                }
            }

            // Check for sidebar elements
            var sidebarElements = await page.QuerySelectorAllAsync("[class*='sidebar'], [class*='nav'], [id*='sidebar'], [id*='nav']");
            Console.WriteLine($"\nFound {sidebarElements.Count} sidebar/nav related elements");

            // Look for the specific NavMenu component we saw in the code
            Console.WriteLine("\n=== CHECKING FOR STYLED NAVIGATION ELEMENTS ===");
            var brandSection = await page.QuerySelectorAsync("#nav-brand-section");
            Console.WriteLine($"Brand section (#nav-brand-section): {brandSection != null}");

            var navFooter = await page.QuerySelectorAsync("#nav-footer");
            Console.WriteLine($"Nav footer (#nav-footer): {navFooter != null}");

            // Check if there's a different layout being used
            var layoutDivs = await page.QuerySelectorAllAsync("div[class*='layout'], div[class*='main']");
            Console.WriteLine($"\nLayout divs found: {layoutDivs.Count}");

            // Summary
            Console.WriteLine("\n=== SUMMARY ===");
            Console.WriteLine($"Total visible navigation links: {navTexts.Count}");
            Console.WriteLine($"Navigation link texts: {string.Join(", ", navTexts)}");
            Console.WriteLine($"Recipes link present: {navTexts.Any(t => t.Contains("Recipes", StringComparison.OrdinalIgnoreCase))}");
            Console.WriteLine($"Styled navigation menu (#nav-menu) present: {styledNavMenu != null}");

            // Save a screenshot of just the sidebar area
            var sidebar = await page.QuerySelectorAsync("nav, .sidebar, aside");
            if (sidebar != null)
            {
                await sidebar.ScreenshotAsync(new ElementHandleScreenshotOptions
                {
                    Path = @"D:\KimiTest\RestoranKuzma\playwright_test\admin_sidebar.png"
                });
                Console.WriteLine("\nSidebar screenshot saved.");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            // Take an error screenshot
            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = @"D:\KimiTest\RestoranKuzma\playwright_test\admin_error.png",
                FullPage = true
            });
            Console.WriteLine("Error screenshot saved.");
        }
    }
}
