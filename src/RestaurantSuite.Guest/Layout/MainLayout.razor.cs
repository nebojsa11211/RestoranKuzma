using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using RestaurantSuite.Guest.Services;

namespace RestaurantSuite.Guest.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private MobileInteractionService MobileService { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    private bool userMenuOpen = false;
    private bool mobileMenuOpen = false;
    private bool isLoggedIn = false;
    private string? userName = "Guest";
    private bool IsScrolled = false;
    private bool HasBottomNav => true; // Bottom nav is always visible on mobile

    // Navigation routes for swipe gestures
    private readonly string[] _navigationRoutes = new[]
    {
        "",           // Home
        "menu",       // Menu
        "order",      // Order
        "tables",     // Tables
    };

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        isLoggedIn = authState.User.Identity?.IsAuthenticated ?? false;

        if (isLoggedIn)
        {
            // Get user name from claims
            userName = authState.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value
                      ?? authState.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                      ?? "Guest";
        }

        // Subscribe to authentication state changes
        AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        var authState = await task;
        isLoggedIn = authState.User.Identity?.IsAuthenticated ?? false;

        if (isLoggedIn)
        {
            userName = authState.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value
                      ?? authState.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                      ?? "Guest";
        }
        else
        {
            userName = "Guest";
        }

        await InvokeAsync(StateHasChanged);
    }

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
        // Sidebar is visible on desktop (768px+), hidden on mobile (uses bottom nav instead)
        return "desktop-sidebar flex-col fixed top-0 left-0 bottom-0 z-30 w-64 bg-white border-r border-neutral-200 shadow-soft-md overflow-y-auto scrollbar-thin";
    }

    private void ToggleUserMenu()
    {
        userMenuOpen = !userMenuOpen;
    }

    private void ToggleMobileMenu()
    {
        mobileMenuOpen = !mobileMenuOpen;
    }

    private void CloseMobileMenu()
    {
        mobileMenuOpen = false;
    }

    private void CloseUserMenu()
    {
        userMenuOpen = false;
    }

    private void NavigateToMyOrders()
    {
        userMenuOpen = false;
        NavigationManager.NavigateTo("/myorders");
    }

    private void NavigateToProfile()
    {
        userMenuOpen = false;
        NavigationManager.NavigateTo("/profile");
    }

    private void NavigateToReservations()
    {
        userMenuOpen = false;
        NavigationManager.NavigateTo("/tables");
    }

    private void SignIn()
    {
        userMenuOpen = false;
        mobileMenuOpen = false;
        NavigationManager.NavigateTo("/login");
    }

    private void Register()
    {
        userMenuOpen = false;
        mobileMenuOpen = false;
        NavigationManager.NavigateTo("/register");
    }

    private void SignOut()
    {
        userMenuOpen = false;
        mobileMenuOpen = false;
        NavigationManager.NavigateTo("/logout");
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            MobileService.OnSwipeDetected -= HandleSwipe;
            await MobileService.DisposeSwipeGesturesAsync();
            AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disposing MainLayout: {ex.Message}");
        }
    }
}
