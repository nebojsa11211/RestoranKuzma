using Microsoft.JSInterop;

namespace RestaurantSuite.Guest.Services;

/// <summary>
/// Service for mobile interactions including haptic feedback, pull-to-refresh, and swipe gestures
/// </summary>
public class MobileInteractionService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;
    private DotNetObjectReference<MobileInteractionService>? _dotNetReference;

    public MobileInteractionService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task EnsureModuleAsync()
    {
        if (_module == null)
        {
            // Module is loaded from inline script in index.html
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("eval", "window.mobileInteractions");
        }
    }

    // ===================================
    // HAPTIC FEEDBACK
    // ===================================

    /// <summary>
    /// Trigger haptic feedback
    /// </summary>
    /// <param name="type">Type of haptic: light, medium, heavy, success, warning, error</param>
    public async Task HapticAsync(string type = "light")
    {
        try
        {
            await EnsureModuleAsync();
            if (_module != null)
            {
                await _module.InvokeVoidAsync("haptic", type);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Haptic] Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Initialize haptic feedback for all buttons
    /// </summary>
    public async Task InitializeButtonHapticsAsync()
    {
        try
        {
            await EnsureModuleAsync();
            if (_module != null)
            {
                await _module.InvokeVoidAsync("initializeButtonHaptics");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Haptic] Error initializing: {ex.Message}");
        }
    }

    // ===================================
    // PULL-TO-REFRESH
    // ===================================

    /// <summary>
    /// Callback for pull-to-refresh action
    /// </summary>
    public event Func<Task>? OnPullToRefreshRequested;

    /// <summary>
    /// Initialize pull-to-refresh on an element
    /// </summary>
    /// <param name="elementId">ID of the scrollable element</param>
    public async Task<bool> InitializePullToRefreshAsync(string elementId)
    {
        try
        {
            await EnsureModuleAsync();
            if (_module != null)
            {
                _dotNetReference = DotNetObjectReference.Create(this);
                return await _module.InvokeAsync<bool>("initializePullToRefresh", elementId, _dotNetReference);
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PTR] Error initializing: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// JavaScript callback for pull-to-refresh
    /// </summary>
    [JSInvokable]
    public async Task OnPullToRefresh()
    {
        try
        {
            if (OnPullToRefreshRequested != null)
            {
                await OnPullToRefreshRequested.Invoke();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PTR] Error in callback: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Dispose pull-to-refresh
    /// </summary>
    public async Task DisposePullToRefreshAsync()
    {
        try
        {
            await EnsureModuleAsync();
            if (_module != null)
            {
                await _module.InvokeVoidAsync("disposePullToRefresh");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PTR] Error disposing: {ex.Message}");
        }
    }

    // ===================================
    // SWIPE GESTURES
    // ===================================

    /// <summary>
    /// Callback for swipe gestures
    /// </summary>
    public event Func<string, Task>? OnSwipeDetected;

    /// <summary>
    /// Initialize swipe gesture detection
    /// </summary>
    /// <param name="elementId">ID of the element (null for body)</param>
    public async Task<bool> InitializeSwipeGesturesAsync(string? elementId = null)
    {
        try
        {
            await EnsureModuleAsync();
            if (_module != null)
            {
                _dotNetReference ??= DotNetObjectReference.Create(this);
                return await _module.InvokeAsync<bool>("initializeSwipeGestures", elementId, _dotNetReference);
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Swipe] Error initializing: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// JavaScript callback for swipe detection
    /// </summary>
    [JSInvokable]
    public async Task OnSwipe(string direction)
    {
        try
        {
            Console.WriteLine($"[Swipe] Detected: {direction}");

            if (OnSwipeDetected != null)
            {
                await OnSwipeDetected.Invoke(direction);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Swipe] Error in callback: {ex.Message}");
        }
    }

    /// <summary>
    /// Dispose swipe gestures
    /// </summary>
    public async Task DisposeSwipeGesturesAsync()
    {
        try
        {
            await EnsureModuleAsync();
            if (_module != null)
            {
                await _module.InvokeVoidAsync("disposeSwipeGestures");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Swipe] Error disposing: {ex.Message}");
        }
    }

    // ===================================
    // DISPOSAL
    // ===================================

    public async ValueTask DisposeAsync()
    {
        try
        {
            await DisposePullToRefreshAsync();
            await DisposeSwipeGesturesAsync();

            _dotNetReference?.Dispose();

            if (_module != null)
            {
                await _module.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Mobile Interactions] Error disposing: {ex.Message}");
        }
    }
}
