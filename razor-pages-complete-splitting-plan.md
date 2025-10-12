# Complete Razor Pages Splitting Plan - ALL PAGES

## Required Structure for EVERY Page
1. **.razor** - HTML markup only
2. **.razor.css** - CSS styles only  
3. **.razor.cs** - C# code only

## ALL Pages to Split (Complete List)

### RestaurantSuite.Admin
- [ ] Counter.razor (needs CSS file)
- [ ] FetchData.razor (needs CSS file)
- [ ] Index.razor (needs CSS file)
- [ ] Orders.razor (needs all 3 files)
- [ ] Reports.razor (needs all 3 files)
- [ ] Settings.razor (needs all 3 files)
- [ ] Staff.razor (needs all 3 files)
- [ ] Tables.razor (needs all 3 files)
- [ ] TestButton.razor (needs all 3 files)

### RestaurantSuite.Chef
- [ ] Counter.razor (needs all 3 files)
- [ ] Home.razor (needs all 3 files)
- [ ] Weather.razor (needs all 3 files)

### RestaurantSuite.Guest
- [ ] Counter.razor (needs all 3 files)
- [ ] Home.razor (needs CSS file)
- [ ] Menu.razor (needs all 3 files)
- [ ] Order.razor (needs all 3 files)
- [ ] ReserveTable.razor (needs all 3 files)
- [ ] Tables.razor (needs all 3 files)
- [ ] Weather.razor (needs all 3 files)

### RestaurantSuite.Waiter
- [ ] Counter.razor (needs all 3 files)
- [ ] Home.razor (needs all 3 files)
- [ ] Weather.razor (needs all 3 files)

## Extraction Strategy
1. **Extract all inline CSS/Tailwind classes** to .razor.css files
2. **Extract all C# code blocks** to .razor.cs files
3. **Leave only clean HTML** in .razor files
4. **Preserve all functionality** but separate concerns completely
