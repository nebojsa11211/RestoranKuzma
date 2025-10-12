# Razor Pages Splitting Plan

## Current Structure Analysis
Most Razor pages contain:
- HTML markup with embedded Tailwind CSS classes
- C# code in `@code` blocks
- No separate CSS files (relying on Tailwind utility classes)

## Splitting Strategy
For each page, we'll create:
1. **.razor** - HTML markup only
2. **.razor.cs** - C# code-behind (if complex logic exists)
3. **.razor.css** - CSS styles (if custom CSS beyond Tailwind is needed)

## Pages to Process

### RestaurantSuite.Admin
- [ ] Counter.razor (simple - may not need splitting)
- [ ] FetchData.razor (check complexity)
- [ ] Index.razor (complex - needs splitting)
- [ ] Orders.razor (complex - needs splitting)
- [ ] Reports.razor (complex - needs splitting)
- [ ] Settings.razor (complex - needs splitting)
- [ ] Staff.razor (simple - may not need splitting)
- [ ] Tables.razor (complex - needs splitting)
- [ ] TestButton.razor (simple - may not need splitting)

### RestaurantSuite.Chef
- [ ] Counter.razor (simple)
- [ ] Home.razor (simple)
- [ ] Weather.razor (simple)

### RestaurantSuite.Guest
- [ ] Counter.razor (simple)
- [ ] Home.razor (complex - needs splitting)
- [ ] Menu.razor (complex - needs splitting)
- [ ] Order.razor (complex - needs splitting)
- [ ] ReserveTable.razor (complex - needs splitting)
- [ ] Tables.razor (complex - needs splitting)
- [ ] Weather.razor (simple)

### RestaurantSuite.Waiter
- [ ] Counter.razor (simple)
- [ ] Home.razor (simple)
- [ ] Weather.razor (simple)

## Approach
1. Start with simple pages to establish pattern
2. Handle complex pages that need significant code separation
3. Preserve existing Tailwind CSS classes in HTML
4. Move C# logic to code-behind files
5. Create CSS files only when custom styling is needed beyond Tailwind
