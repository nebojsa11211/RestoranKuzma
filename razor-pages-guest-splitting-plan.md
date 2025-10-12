# RestaurantSuite.Guest Pages Splitting Plan

## Pages to Split (8 total):
- [ ] Counter.razor (needs all 3 files)
- [ ] Home.razor (needs CSS file - code already exists)
- [ ] Menu.razor (needs all 3 files)
- [ ] Order.razor (needs all 3 files)
- [ ] ReserveTable.razor (needs all 3 files)
- [ ] Tables.razor (needs all 3 files)
- [ ] Weather.razor (needs all 3 files)

## Current Status:
- Home.razor.cs already exists (code-behind done)
- All other pages need complete 3-file splitting

## Approach:
1. Start with simpler pages (Counter, Weather)
2. Handle complex pages (Menu, Order, ReserveTable, Tables)
3. Complete Home.razor by adding CSS file
4. Extract all inline Tailwind classes to CSS
5. Move all C# logic to code-behind
6. Create clean HTML-only Razor files
