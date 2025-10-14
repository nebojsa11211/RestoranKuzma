# Visual Studio Debug Configuration Guide

## Problem
You're seeing a `Microsoft.WebAssembly.AppHost.CommandLineException` error when trying to debug the solution.

## Solution: Configure Multiple Startup Projects

Follow these steps to properly debug the API and Chef projects together in Visual Studio:

### Step 1: Configure Multiple Startup Projects

1. **Right-click on the solution** (RestaurantSuite) in Solution Explorer
2. Select **"Configure Startup Projects..."**
3. Choose **"Multiple startup projects"**
4. Set the following projects:

   | Project | Action | Details |
   |---------|--------|---------|
   | **RestaurantSuite.Api** | **Start** | Backend API server |
   | **RestaurantSuite.Chef** | **Start** | Blazor WebAssembly client |
   | All other projects | None | (Leave as "None") |

5. Click **OK**

### Step 2: Verify Launch Settings

#### API Launch Settings
File: `src/RestaurantSuite.Api/Properties/launchSettings.json`

The API should start on:
- HTTP: http://localhost:5213
- Swagger: http://localhost:5213/swagger

#### Chef Launch Settings
File: `src/RestaurantSuite.Chef/Properties/launchSettings.json`

The Chef app should start on:
- HTTPS: https://localhost:7046
- HTTP: http://localhost:5173

### Step 3: Set Breakpoints and Debug

1. Set breakpoints in your code (API controllers, Blazor components, etc.)
2. Press **F5** or click **"Start Debugging"**
3. Both projects will start simultaneously:
   - API will listen on port 5213
   - Chef app will open in your browser on port 7046

### Alternative: Debug Single Project

If you only want to debug one project at a time:

1. **Right-click on the solution** in Solution Explorer
2. Select **"Configure Startup Projects..."**
3. Choose **"Single startup project"**
4. Select either:
   - **RestaurantSuite.Api** (for backend debugging)
   - **RestaurantSuite.Chef** (for frontend debugging)

### Troubleshooting

#### Issue: WebAssembly CommandLineException

**Cause:** Visual Studio is trying to launch incompatible projects together or using incorrect launch profiles.

**Solution:**
1. Close Visual Studio
2. Delete the `.vs` folder in the solution root (it contains cache)
3. Reopen Visual Studio
4. Reconfigure startup projects as described above

#### Issue: Port Already in Use

**Cause:** A previous instance is still running.

**Solution:**
1. Stop all debugging sessions (Shift+F5)
2. Check Task Manager for any lingering `dotnet.exe` processes
3. Kill any dotnet processes if found
4. Restart debugging

#### Issue: Chef App Cannot Connect to API

**Cause:** API URL mismatch in configuration.

**Solution:**
Check `src/RestaurantSuite.Chef/wwwroot/appsettings.json` and ensure:
```json
{
  "ApiBaseUrl": "http://localhost:5213"
}
```

### Running Other Blazor Applications

To debug other applications (Admin, Guest, Waiter), change the startup projects:

#### Admin Application
- Set **RestaurantSuite.Api** and **RestaurantSuite.Admin** to Start
- Admin runs on: https://localhost:7220

#### Guest Application
- Set **RestaurantSuite.Api** and **RestaurantSuite.Guest** to Start
- Guest runs on: https://localhost:7140

#### Waiter Application
- Set **RestaurantSuite.Api** and **RestaurantSuite.Waiter** to Start
- Waiter runs on: https://localhost:7030

### Quick Start Commands (Alternative to Visual Studio)

If you prefer command line:

```bash
# Terminal 1: Start API
cd src/RestaurantSuite.Api
dotnet run

# Terminal 2: Start Chef
cd src/RestaurantSuite.Chef
dotnet run
```

Then open https://localhost:7046 in your browser.

### Visual Studio Launch Profiles

You can also create custom launch profiles:

1. Right-click the solution
2. Select **"Manage Launch Profiles"**
3. Create a new profile named "API + Chef"
4. Configure it to start both projects

This profile will be saved in `.vs/` folder and available in the dropdown next to the Start button.

---

## Current Running Instances

Currently, the following are running from command line:
- API: http://localhost:5213 ✅
- Admin: https://localhost:7220 ✅

You can stop these and start from Visual Studio for debugging.
