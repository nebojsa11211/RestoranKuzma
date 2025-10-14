using Microsoft.AspNetCore.Components;
using RestaurantSuite.Guest.Services;

namespace RestaurantSuite.Guest.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private MobileInteractionService MobileService { get; set; } = default!;

    // Navigation routes for swipe gestures
    private readonly string[] _navigationRoutes = new[]
    {
        "",           // Home
        "menu",       // Menu
        "order",      // Order
        "tables",     // Tables
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Initialize swipe gestures
            MobileService.OnSwipeDetected += HandleSwipe;
            await MobileService.InitializeSwipeGesturesAsync("main-content");
            Console.WriteLine("✅ Swipe gestures initialized on MainLayout");
        }
    }

    private async Task HandleSwipe(string direction)
    {
        Console.WriteLine($"🔄 Swipe detected: {direction}");

        // Get current path
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var currentPath = uri.AbsolutePath.TrimStart('/').ToLower();

        // Find current index
        int currentIndex = Array.FindIndex(_navigationRoutes, r => r == currentPath);
        if (currentIndex == -1)
        {
            currentIndex = 0; // Default to home if not found
        }

        // Navigate based on swipe direction
        switch (direction)
        {
            case "left":
                // Swipe left = next page
                if (currentIndex < _navigationRoutes.Length - 1)
                {
                    var nextRoute = _navigationRoutes[currentIndex + 1];
                    Console.WriteLine($"Navigate to: /{nextRoute}");
                    NavigationManager.NavigateTo($"/{nextRoute}");
                    await MobileService.HapticAsync("light");
                }
                break;

            case "right":
                // Swipe right = previous page
                if (currentIndex > 0)
                {
                    var prevRoute = _navigationRoutes[currentIndex - 1];
                    Console.WriteLine($"Navigate to: /{prevRoute}");
                    NavigationManager.NavigateTo($"/{prevRoute}");
                    await MobileService.HapticAsync("light");
                }
                break;
        }
    }

    private string GetSidebarClasses()
    {
        // Sidebar is hidden on mobile (uses bottom nav instead) and visible on desktop (lg+)
        return "hidden lg:flex fixed top-0 left-0 bottom-0 z-30 w-64 bg-white border-r border-neutral-200 shadow-soft-md overflow-y-auto scrollbar-thin";
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            MobileService.OnSwipeDetected -= HandleSwipe;
            await MobileService.DisposeSwipeGesturesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disposing MainLayout: {ex.Message}");
        }
    }
}
