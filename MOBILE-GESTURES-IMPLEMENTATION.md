# Mobile Gestures Implementation Guide
## Pull-to-Refresh, Swipe Navigation, and Haptic Feedback

**Implementation Date:** 2025-10-14
**Project:** Restaurant Suite - Guest Portal
**Features:** Pull-to-Refresh, Swipe Gestures, Haptic Feedback

---

## Overview

This document describes the implementation of three high-priority mobile interaction features:

1. **Pull-to-Refresh** - Users can pull down on the menu page to refresh content
2. **Swipe Gestures** - Users can swipe left/right to navigate between pages
3. **Haptic Feedback** - Tactile feedback on button interactions and gestures

---

## Architecture

### Technology Stack
- **Frontend Framework:** Blazor WebAssembly (.NET 9)
- **JavaScript Interop:** Custom mobile-interactions.js library
- **C# Service:** MobileInteractionService
- **CSS:** Enhanced mobile.css with pull-to-refresh styles

### Component Interaction Flow
```
User Action → JavaScript Detection → C# Callback → App Logic → UI Update
```

---

## Implementation Details

### 1. JavaScript Library (`mobile-interactions.js`)

Located at: `wwwroot/js/mobile-interactions.js`

#### Features Provided:
- **Haptic Feedback API**
  - Light, medium, heavy vibrations
  - Success, warning, error patterns
  - Automatic button initialization

- **Pull-to-Refresh**
  - Touch event handling
  - Visual indicator with animations
  - Resistance-based pulling
  - State management (pulling, ready, loading, success, error)

- **Swipe Gesture Detection**
  - Touch start/end tracking
  - Direction calculation
  - Minimum distance threshold
  - Maximum time constraint

#### Key Functions:
```javascript
// Haptic feedback
window.mobileInteractions.haptic(type)

// Pull-to-refresh
window.mobileInteractions.initializePullToRefresh(elementId, dotnetHelper)

// Swipe gestures
window.mobileInteractions.initializeSwipeGestures(elementId, dotnetHelper)
```

---

### 2. C# Interop Service

Located at: `Services/MobileInteractionService.cs`

#### Purpose:
Provides a C# interface to JavaScript mobile interaction features

#### Key Methods:

**Haptic Feedback:**
```csharp
await MobileService.HapticAsync("success");
```

**Pull-to-Refresh:**
```csharp
// Initialize
await MobileService.InitializePullToRefreshAsync("elementId");

// Handle callback
MobileService.OnPullToRefreshRequested += async () => {
    // Refresh logic here
};
```

**Swipe Gestures:**
```csharp
// Initialize
await MobileService.InitializeSwipeGesturesAsync("elementId");

// Handle callback
MobileService.OnSwipeDetected += async (direction) => {
    // Navigation logic here
};
```

#### Lifecycle:
- Service is registered as **Scoped** in DI container
- Implements `IAsyncDisposable` for proper cleanup
- Event handlers must be unregistered on disposal

---

### 3. Pull-to-Refresh Implementation

#### Where Implemented:
- **Menu Page** (`Pages/Menu.razor.cs`)

#### How It Works:

1. **Initialization** (OnAfterRender):
```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        MobileService.OnPullToRefreshRequested += HandleRefresh;
        await MobileService.InitializePullToRefreshAsync("menu-page-container");
    }
}
```

2. **Refresh Handler**:
```csharp
private async Task HandleRefresh()
{
    await MobileService.HapticAsync("light");

    try
    {
        await LoadCategories();
        await LoadMenuItems();
        StateHasChanged();
    }
    catch (Exception ex)
    {
        await MobileService.HapticAsync("error");
        throw;
    }
}
```

3. **Cleanup** (Disposal):
```csharp
public async ValueTask DisposeAsync()
{
    MobileService.OnPullToRefreshRequested -= HandleRefresh;
    await MobileService.DisposePullToRefreshAsync();
}
```

#### Visual Indicator States:

| State | Color | Icon | Description |
|-------|-------|------|-------------|
| **Pulling** | Gray | Refresh icon rotating | User is pulling down |
| **Ready** | Blue | Refresh icon 180° | Threshold reached, ready to refresh |
| **Loading** | Blue | Spinning icon | Refresh in progress |
| **Success** | Green | Checkmark | Refresh completed |
| **Error** | Red | Error icon | Refresh failed |

---

### 4. Swipe Gesture Navigation

#### Where Implemented:
- **MainLayout** (`Layout/MainLayout.razor.cs`)

#### Navigation Flow:

```
Home → Menu → Order → Tables
```

- **Swipe Left:** Navigate to next page
- **Swipe Right:** Navigate to previous page

#### Implementation:

1. **Route Configuration**:
```csharp
private readonly string[] _navigationRoutes = new[]
{
    "",           // Home
    "menu",       // Menu
    "order",      // Order
    "tables",     // Tables
};
```

2. **Swipe Handler**:
```csharp
private async Task HandleSwipe(string direction)
{
    var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
    var currentPath = uri.AbsolutePath.TrimStart('/').ToLower();

    int currentIndex = Array.FindIndex(_navigationRoutes, r => r == currentPath);

    switch (direction)
    {
        case "left":
            if (currentIndex < _navigationRoutes.Length - 1)
            {
                NavigationManager.NavigateTo($"/{_navigationRoutes[currentIndex + 1]}");
                await MobileService.HapticAsync("light");
            }
            break;

        case "right":
            if (currentIndex > 0)
            {
                NavigationManager.NavigateTo($"/{_navigationRoutes[currentIndex - 1]}");
                await MobileService.HapticAsync("light");
            }
            break;
    }
}
```

3. **Gesture Parameters**:
- **Minimum Distance:** 50px
- **Maximum Time:** 500ms
- **Direction Detection:** Compares horizontal vs vertical delta

---

### 5. Haptic Feedback

#### Where Implemented:
- **All buttons** (automatic via MutationObserver)
- **Menu page** - "Add to Order" buttons
- **Swipe navigation** - Page transitions
- **Pull-to-refresh** - Gesture feedback

#### Vibration Patterns:

| Type | Pattern | Use Case |
|------|---------|----------|
| **light** | [10ms] | Button clicks, light feedback |
| **medium** | [20ms] | Moderate actions |
| **heavy** | [30ms] | Important actions |
| **success** | [10, 50, 10] | Successful operations |
| **warning** | [15, 100, 15] | Warning alerts |
| **error** | [30, 100, 30, 100, 30] | Error states |
| **selection** | [5ms] | Item selection |

#### Browser Support:
- **Supported:** Chrome/Edge (Android), Safari (iOS 13+)
- **Fallback:** Gracefully degrades, no errors on unsupported browsers
- **API Used:** `navigator.vibrate()`

---

## CSS Styling

### Pull-to-Refresh Indicator

Located in: `wwwroot/css/mobile.css`

```css
.pull-to-refresh-indicator {
    position: fixed;
    top: 0;
    left: 50%;
    transform: translateX(-50%) translateY(-60px);
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8px;
    padding: 12px 20px;
    background: white;
    border-radius: 0 0 12px 12px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    z-index: 1000;
    opacity: 0;
    transition: transform 0.2s ease, opacity 0.2s ease;
}
```

#### State Classes:
- `.ptr-ready` - Blue background when ready to refresh
- `.ptr-loading` - Spinning animation
- `.ptr-success` - Green background on success
- `.ptr-error` - Red background on error

---

## Usage Guide

### Adding Pull-to-Refresh to a New Page

1. **Inject the service** in your component:
```csharp
[Inject]
private MobileInteractionService MobileService { get; set; } = default!;
```

2. **Implement IAsyncDisposable**:
```csharp
public partial class YourPage : IAsyncDisposable
```

3. **Initialize in OnAfterRenderAsync**:
```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        MobileService.OnPullToRefreshRequested += HandleRefresh;
        await MobileService.InitializePullToRefreshAsync("your-container-id");
    }
}
```

4. **Implement refresh handler**:
```csharp
private async Task HandleRefresh()
{
    await MobileService.HapticAsync("light");
    // Your refresh logic
    StateHasChanged();
}
```

5. **Cleanup**:
```csharp
public async ValueTask DisposeAsync()
{
    MobileService.OnPullToRefreshRequested -= HandleRefresh;
    await MobileService.DisposePullToRefreshAsync();
}
```

---

### Adding Haptic Feedback to Buttons

**Automatic (Recommended):**
All buttons get automatic haptic feedback via the MutationObserver in `mobile-interactions.js`.

**Manual:**
```csharp
private async Task OnButtonClick()
{
    await MobileService.HapticAsync("success");
    // Your button logic
}
```

---

### Adding Swipe Navigation to a Page

Follow the same pattern as MainLayout:

1. Initialize swipe gestures in OnAfterRenderAsync
2. Handle OnSwipeDetected event
3. Implement navigation logic
4. Cleanup on disposal

---

## Testing Guide

### Pull-to-Refresh Testing

#### Desktop (Chrome DevTools)
1. Open DevTools (F12)
2. Enable Device Mode (Ctrl+Shift+M)
3. Select mobile device (iPhone, Pixel, etc.)
4. Navigate to Menu page
5. Click and drag down from top
6. Release when indicator shows "Release to refresh"
7. Verify menu reloads

#### Mobile Device
1. Deploy app to server or use tunnel
2. Access from mobile device
3. Navigate to Menu page
4. Pull down from top of page
5. Observe visual feedback
6. Feel haptic vibration
7. Verify content refreshes

### Swipe Gesture Testing

#### Desktop (Chrome DevTools)
1. Enable Device Mode
2. Navigate between pages
3. Click and drag left/right
4. Verify page navigation

#### Mobile Device
1. Access app on mobile
2. Swipe left on Home → navigates to Menu
3. Swipe left on Menu → navigates to Order
4. Swipe right on Menu → navigates to Home
5. Verify haptic feedback on each swipe

### Haptic Feedback Testing

#### Requirements:
- Physical mobile device
- Browser with Vibration API support

#### Test Steps:
1. Click any button
2. Feel short vibration
3. Add item to order
4. Feel success pattern vibration
5. Swipe between pages
6. Feel navigation vibration

---

## Performance Considerations

### JavaScript Performance
- **Event Throttling:** Touch events use passive listeners where possible
- **Mutation Observer:** Batched to avoid excessive DOM queries
- **Memory Management:** Event listeners properly removed on disposal

### C# Performance
- **Service Lifetime:** Scoped to component lifecycle
- **Event Handlers:** Unregistered on disposal to prevent memory leaks
- **Async Operations:** All JS interop is async to prevent blocking

### CSS Performance
- **Hardware Acceleration:** Uses `transform` for animations
- **GPU Rendering:** `translateZ(0)` forces GPU acceleration
- **Efficient Transitions:** Uses `opacity` and `transform` only

---

## Browser Compatibility

| Feature | Chrome (Android) | Safari (iOS) | Firefox (Android) | Samsung Internet |
|---------|------------------|--------------|-------------------|------------------|
| Pull-to-Refresh | ✅ Full | ✅ Full | ✅ Full | ✅ Full |
| Swipe Gestures | ✅ Full | ✅ Full | ✅ Full | ✅ Full |
| Haptic Feedback | ✅ Full | ⚠️ Limited* | ✅ Full | ✅ Full |

**iOS Haptic Limitations:**
- Requires iOS 13+ for Vibration API
- Pattern support is simplified (duration only, no pauses)
- Some patterns may feel similar

---

## Troubleshooting

### Pull-to-Refresh Not Working

**Problem:** Indicator doesn't appear when pulling down

**Solutions:**
1. Ensure element ID matches: `"menu-page-container"`
2. Check browser console for JavaScript errors
3. Verify service is injected: `MobileService`
4. Confirm OnAfterRenderAsync is called
5. Check if page is scrolled to top (PTR only works at scroll position 0)

---

### Swipe Gestures Not Detected

**Problem:** Swiping doesn't navigate

**Solutions:**
1. Verify swipe distance is > 50px
2. Ensure swipe is mostly horizontal (not diagonal)
3. Complete swipe within 500ms
4. Check browser console for errors
5. Confirm routes are in navigation array

---

### No Haptic Feedback

**Problem:** Device doesn't vibrate

**Solutions:**
1. Test on physical device (emulators don't vibrate)
2. Check device vibration settings
3. Verify browser supports Vibration API
4. Test in Chrome/Safari (best support)
5. Check console for "Vibration API not supported" messages

---

## File Changes Summary

### New Files Created:
```
wwwroot/
├── js/
│   └── mobile-interactions.js      (New - JS library)
Services/
└── MobileInteractionService.cs     (New - C# interop)
Layout/
└── MainLayout.razor.cs              (New - Code-behind)
```

### Modified Files:
```
wwwroot/
├── index.html                       (Added script reference)
└── css/mobile.css                   (Added PTR styles)
Pages/
└── Menu.razor.cs                    (Added PTR + haptics)
Layout/
└── MainLayout.razor                 (Removed inline code)
Program.cs                           (Registered service)
```

---

## Future Enhancements

### Potential Improvements:
1. **Customizable Pull-to-Refresh**
   - Allow custom refresh thresholds
   - Configurable indicator styles
   - Custom refresh animations

2. **Advanced Swipe Gestures**
   - Vertical swipe actions
   - Multi-finger gestures
   - Gesture customization per page

3. **Enhanced Haptics**
   - iOS Taptic Engine integration
   - Android vibration effects API
   - Customizable haptic patterns

4. **Gesture Analytics**
   - Track gesture usage
   - Identify popular navigation paths
   - Optimize gesture thresholds based on data

---

## API Reference

### MobileInteractionService

#### Methods

**HapticAsync(string type)**
```csharp
await MobileService.HapticAsync("success");
```
- **Parameters:** "light", "medium", "heavy", "success", "warning", "error"
- **Returns:** Task
- **Description:** Triggers device vibration

**InitializePullToRefreshAsync(string elementId)**
```csharp
bool success = await MobileService.InitializePullToRefreshAsync("container-id");
```
- **Parameters:** HTML element ID
- **Returns:** Task\<bool>
- **Description:** Enables pull-to-refresh on element

**InitializeSwipeGesturesAsync(string? elementId)**
```csharp
bool success = await MobileService.InitializeSwipeGesturesAsync("container-id");
```
- **Parameters:** HTML element ID (null for body)
- **Returns:** Task\<bool>
- **Description:** Enables swipe detection

#### Events

**OnPullToRefreshRequested**
```csharp
MobileService.OnPullToRefreshRequested += async () => {
    // Refresh logic
};
```
- **Type:** Func\<Task>
- **Description:** Fired when user completes pull-to-refresh

**OnSwipeDetected**
```csharp
MobileService.OnSwipeDetected += async (direction) => {
    // Navigation logic
};
```
- **Type:** Func\<string, Task>
- **Parameters:** "left", "right", "up", "down"
- **Description:** Fired when swipe gesture detected

---

## Best Practices

### Do's ✅
- Always implement IAsyncDisposable when using mobile services
- Unregister event handlers in DisposeAsync
- Use haptic feedback sparingly for important actions
- Test on real devices, not just emulators
- Provide visual feedback along with haptic
- Initialize gestures in OnAfterRenderAsync with firstRender check

### Don'ts ❌
- Don't forget to dispose services
- Don't use haptics for every single interaction
- Don't rely solely on haptics (provide visual feedback)
- Don't initialize multiple times
- Don't test haptics only on desktop
- Don't use complex haptic patterns excessively (drains battery)

---

## Conclusion

These mobile interaction features significantly enhance the user experience on mobile devices by providing:

1. **Natural Interactions** - Familiar mobile gestures
2. **Tactile Feedback** - Physical confirmation of actions
3. **Efficient Navigation** - Quick page transitions
4. **Content Freshness** - Easy data refresh

The implementation follows mobile-first principles and provides a native app-like experience while maintaining web platform benefits.

---

**Document Version:** 1.0
**Last Updated:** 2025-10-14
**Author:** Claude Code with mobile-blazor-ux-expert agent
**License:** Internal Documentation
