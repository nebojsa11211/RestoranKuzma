# FIX: Visual Studio Debug Error

## The Problem
```
Exception thrown: 'Microsoft.WebAssembly.AppHost.CommandLineException' in WasmAppHost.dll
The program has exited with code 4294967295 (0xffffffff)
```

This error occurs because Visual Studio 2022 has special handling for Blazor WebAssembly projects that conflicts with multi-project debugging.

## ✅ WORKING SOLUTION (Choose ONE)

### Solution A: Debug API Only, Run Chef Separately (EASIEST)

This is the **recommended approach** for daily development:

#### Step 1: Configure Visual Studio
1. In Visual Studio, right-click the **Solution** in Solution Explorer
2. Select **"Set Startup Projects..."**
3. Choose **"Single startup project"**
4. Select: **RestaurantSuite.Api**
5. Click **OK**

#### Step 2: Start API in Visual Studio
1. Press **F5** (or click Start Debugging)
2. The API will start on http://localhost:5213
3. You can set breakpoints in controllers, services, etc.

#### Step 3: Start Chef in Terminal
Open a **new terminal/PowerShell** window:
```powershell
cd D:\KimiTest\RestoranKuzma
cd src\RestaurantSuite.Chef
dotnet watch
```

The Chef app will:
- Start on https://localhost:7046
- Auto-reload when you make changes
- Connect to your API at localhost:5213

**Why this works:**
- API runs in full debug mode in Visual Studio
- Chef runs independently with hot reload
- No WebAssembly host conflicts
- Both can be debugged (API in VS, Chef in browser DevTools)

---

### Solution B: Debug Both in Visual Studio (ADVANCED)

If you absolutely need both in Visual Studio debugger:

#### Step 1: Modify Chef's Launch Settings
Edit: `src\RestaurantSuite.Chef\Properties\launchSettings.json`

Change from:
```json
{
  "commandName": "Project",
  ...
}
```

To:
```json
{
  "commandName": "Executable",
  "executablePath": "dotnet",
  "commandLineArgs": "run --no-build",
  "workingDirectory": "$(ProjectDir)",
  ...
}
```

#### Step 2: Configure Multiple Startup Projects
1. Right-click solution → **"Set Startup Projects..."**
2. Select **"Multiple startup projects"**
3. Set:
   - **RestaurantSuite.Api** → Start
   - **RestaurantSuite.Chef** → Start
4. Click **OK**

#### Step 3: Launch
Press **F5**

**Note:** This approach is more complex and may still have issues. Solution A is preferred.

---

### Solution C: Use VS Code Instead (ALTERNATIVE)

If Visual Studio continues to have issues, VS Code works perfectly:

#### Step 1: Open in VS Code
```powershell
cd D:\KimiTest\RestoranKuzma
code .
```

#### Step 2: Select Debug Configuration
1. Click the Debug icon (or Ctrl+Shift+D)
2. Select **"API + Chef"** from the dropdown
3. Press **F5**

Both projects will start with full debugging support.

---

## Quick Reference

### Current Ports:
- **API**: http://localhost:5213
- **API Swagger**: http://localhost:5213/swagger
- **Chef App**: https://localhost:7046 or http://localhost:5173
- **Admin App**: https://localhost:7220 or http://localhost:5054
- **Guest App**: https://localhost:7140
- **Waiter App**: https://localhost:7030

### Debugging Checklist:
- [ ] Is the API running? (Check http://localhost:5213/swagger)
- [ ] Is the Chef app pointing to correct API? (Check `src/RestaurantSuite.Chef/wwwroot/appsettings.json`)
- [ ] Are there old dotnet.exe processes? (Check Task Manager)
- [ ] Did you try closing and reopening Visual Studio?
- [ ] Did you delete the `.vs` folder? (Close VS first)

### Common Issues:

**Q: Port already in use**
```powershell
# Kill all dotnet processes
Get-Process -Name dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
```

**Q: Chef can't connect to API**
- Verify API is running: http://localhost:5213/swagger
- Check `src/RestaurantSuite.Chef/wwwroot/appsettings.json` has correct URL
- Check browser console (F12) for connection errors

**Q: Changes not reflecting**
- For API: Stop debugging (Shift+F5) and restart (F5)
- For Chef: Use `dotnet watch` instead of `dotnet run` for auto-reload

---

## Recommended Daily Workflow

1. **Morning**:
   - Open Visual Studio
   - Set startup project to **RestaurantSuite.Api**
   - Press F5

2. **Open Terminal**:
   ```powershell
   cd src\RestaurantSuite.Chef
   dotnet watch
   ```

3. **Develop**:
   - Make changes to API → automatically rebuilt by Visual Studio
   - Make changes to Chef → automatically reloaded by `dotnet watch`
   - Set breakpoints in API code
   - Use browser DevTools (F12) for Chef debugging

4. **Evening**:
   - Stop debugging (Shift+F5 in VS)
   - Ctrl+C in terminal to stop Chef

This workflow gives you the best of both worlds with zero configuration hassles!
