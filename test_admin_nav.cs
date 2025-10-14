using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantSuite.Tests
{
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

                // Wait for the navigation menu to be visible
                await page.WaitForSelectorAsync("#nav-menu", new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 10000
                });

                Console.WriteLine("Navigation menu is visible.");

                // Take a screenshot of the entire page
                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = @"D:\KimiTest\RestoranKuzma\admin_nav_full.png",
                    FullPage = true
                });

                Console.WriteLine("Full page screenshot saved to: D:\\KimiTest\\RestoranKuzma\\admin_nav_full.png");

                // Take a focused screenshot of the navigation menu
                var navMenu = await page.QuerySelectorAsync("#nav-menu");
                if (navMenu != null)
                {
                    await navMenu.ScreenshotAsync(new ElementHandleScreenshotOptions
                    {
                        Path = @"D:\KimiTest\RestoranKuzma\admin_nav_menu.png"
                    });
                    Console.WriteLine("Navigation menu screenshot saved to: D:\\KimiTest\\RestoranKuzma\\admin_nav_menu.png");
                }

                // Get all NavLink elements
                var navLinks = await page.QuerySelectorAllAsync("a.group");
                Console.WriteLine($"\nFound {navLinks.Count} navigation links in total.");

                Console.WriteLine("\n=== ALL NAVIGATION LINKS ===");
                var linkTexts = new List<string>();
                for (int i = 0; i < navLinks.Count; i++)
                {
                    var link = navLinks[i];
                    var href = await link.GetAttributeAsync("href");
                    var textContent = await link.TextContentAsync();
                    var isVisible = await link.IsVisibleAsync();

                    linkTexts.Add(textContent?.Trim() ?? "");

                    Console.WriteLine($"{i + 1}. Text: '{textContent?.Trim()}'");
                    Console.WriteLine($"   Href: '{href}'");
                    Console.WriteLine($"   Visible: {isVisible}");
                    Console.WriteLine();
                }

                // Check specifically for the Recipes link
                Console.WriteLine("=== RECIPES LINK VERIFICATION ===");

                // Method 1: Check by href
                var recipesLinkByHref = await page.QuerySelectorAsync("a[href='recipes']");
                Console.WriteLine($"Recipes link found by href='recipes': {recipesLinkByHref != null}");

                if (recipesLinkByHref != null)
                {
                    var isVisible = await recipesLinkByHref.IsVisibleAsync();
                    var boundingBox = await recipesLinkByHref.BoundingBoxAsync();
                    Console.WriteLine($"  - Is visible: {isVisible}");
                    Console.WriteLine($"  - Bounding box: {(boundingBox != null ? $"x={boundingBox.X}, y={boundingBox.Y}, width={boundingBox.Width}, height={boundingBox.Height}" : "null")}");

                    var textContent = await recipesLinkByHref.TextContentAsync();
                    Console.WriteLine($"  - Text content: '{textContent?.Trim()}'");

                    var computedStyle = await page.EvaluateAsync<string>(@"(element) => {
                        const style = window.getComputedStyle(element);
                        return JSON.stringify({
                            display: style.display,
                            visibility: style.visibility,
                            opacity: style.opacity,
                            position: style.position
                        });
                    }", recipesLinkByHref);
                    Console.WriteLine($"  - Computed styles: {computedStyle}");
                }

                // Method 2: Check by text content
                var recipesLinkByText = await page.QuerySelectorAsync("text='Recipes'");
                Console.WriteLine($"\nRecipes link found by text='Recipes': {recipesLinkByText != null}");

                // Method 3: Check the Recipes icon specifically
                var recipesIcon = await page.QuerySelectorAsync("#nav-icon-recipes");
                Console.WriteLine($"Recipes icon found by id='nav-icon-recipes': {recipesIcon != null}");

                if (recipesIcon != null)
                {
                    var isIconVisible = await recipesIcon.IsVisibleAsync();
                    Console.WriteLine($"  - Icon is visible: {isIconVisible}");
                }

                // Get the HTML of the navigation menu
                var navMenuHtml = await page.InnerHTMLAsync("#nav-menu");
                Console.WriteLine("\n=== NAVIGATION MENU HTML (first 2000 chars) ===");
                Console.WriteLine(navMenuHtml.Substring(0, Math.Min(2000, navMenuHtml.Length)));

                // Check if Recipes link exists in the list
                Console.WriteLine("\n=== SUMMARY ===");
                Console.WriteLine($"Total navigation links found: {linkTexts.Count}");
                Console.WriteLine($"Links: {string.Join(", ", linkTexts)}");
                Console.WriteLine($"Recipes link present: {linkTexts.Any(t => t.Contains("Recipes", StringComparison.OrdinalIgnoreCase))}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Take an error screenshot
                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = @"D:\KimiTest\RestoranKuzma\admin_error.png",
                    FullPage = true
                });
                Console.WriteLine("Error screenshot saved to: D:\\KimiTest\\RestoranKuzma\\admin_error.png");
            }
        }
    }
}
