# Project Template Guide

A general-purpose ASP.NET Core Blazor template with authentication, theming, navigation, and API support.

## Quick Start Checklist

- [ ] Rename namespace from `Daisy` to your project name
- [ ] Update `appsettings.json` with your project details
- [ ] Choose authentication mode (None/Cookie/OAuth)
- [ ] Enable/disable feature areas (Dashboard, Settings)
- [ ] Configure theme settings
- [ ] Delete `Areas/Example/` when done
- [ ] Delete this file when done

---

## Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Framework | ASP.NET Core | 10.0 |
| UI | Blazor (Server + SSR) | - |
| Styling | Tailwind CSS + daisyUI | 3.4 / 4.6 |
| Build | .NET SDK + Node.js | - |

---

## Architecture Overview

### Render Mode Strategy

The application uses a **hybrid render mode strategy** to optimize performance:

**Static SSR (Default)** - Most pages for fast initial load:
- `Login.razor`, `Error.razor`, `NotFound.razor`, `AccessDenied.razor`
- `Dashboard/*.razor`, `Settings/*.razor` - Feature pages
- `PageHeader.razor`, `MainLayout.razor`, `EmptyLayout.razor`

**InteractiveServer (SignalR)** - Only for real-time interactivity:
- `TopBar.razor` - Breadcrumb updates on navigation
- `NavMenu.razor` - Sidebar collapse state persistence

**Server-Side Redirect** - Root URL (`/`) via controller:
- `HomeController.cs` - Checks auth, redirects to `/dashboard` or `/login`

### Authentication Flow

```
User visits /
    ↓
HomeController checks auth
    ↓
Authenticated? → /dashboard
Not authenticated? → /login
```

### Performance Optimizations

1. **Minimal InteractiveServer** - Only 2 components use SignalR
2. **Server-side redirects** - No Blazor for simple auth redirects
3. **CSS data attributes** - Prevent FOUC on theme/nav state
4. **Inline critical JS** - Theme/nav state applied before render
5. **Tailwind JIT** - Only used CSS classes in bundle
6. **Conditional StateHasChanged** - Avoid unnecessary re-renders

---

## Configuration Reference

### App Settings (`appsettings.json`)

```json
{
  "App": {
    "Name": "Your App Name"
  },

  "Template": {
    "CookieName": "your-app-auth",
    "CookieExpirationDays": 7,
    "DefaultTheme": "light",
    "LoginPath": "/login",
    "AccessDeniedPath": "/access-denied",
    "DefaultRedirectAfterLogin": "/dashboard",
    "DefaultRedirectAfterLogout": "/"
  },

  "Features": {
    "Dashboard": true,
    "Settings": true
  },

  "Authentication": {
    "Mode": "Cookie",
    "AllowGuestMode": false
  }
}
```

---

## Authentication Modes

### Mode: `None`
No authentication. All pages are public.

```json
"Authentication": {
  "Mode": "None"
}
```

### Mode: `Cookie` (Default)
Cookie-based authentication with built-in login page.

```json
"Authentication": {
  "Mode": "Cookie",
  "AllowGuestMode": true
}
```

**To implement:** Modify `Components/Pages/Login.razor` to validate users against your data source.

### Mode: `OAuth`
OAuth 2.0 authentication via external provider.

```json
"Authentication": {
  "Mode": "OAuth"
},
"Auth0": {
  "Domain": "your-tenant.auth0.com",
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret",
  "CallbackPath": "/auth/callback",
  "SignedOutCallbackPath": "/auth/signout-callback"
}
```

---

## Feature Areas

Feature areas can be enabled/disabled in `appsettings.json`:

```json
"Features": {
  "Dashboard": true,
  "Settings": true
}
```

### Area Structure

```
Areas/YourFeature/
├── YourFeatureController.cs    # API controller + models
└── Views/
    ├── _Imports.razor          # Razor imports
    └── Pages/
        └── Index.razor         # Main page
```

### Creating a New Area

1. Copy `Areas/Example` to `Areas/YourFeature`
2. Update namespaces:
   ```razor
   @namespace Daisy.Areas.YourFeature.Views.Pages
   ```
3. Rename controller:
   ```csharp
   namespace Daisy.Areas.YourFeature;

   [Route("api/[controller]")]
   public class YourFeatureController : ControllerBase
   ```
4. Update `@page` directive:
   ```razor
   @page "/your-feature"
   ```
5. Add menu item in `Program.cs`:
   ```csharp
   if (features.GetValue<bool>("YourFeature", false))
   {
       menuItems.Add(new MenuItem
       {
           Id = "your-feature",
           Title = "Your Feature",
           Href = "/your-feature",
           Icon = "star",
           Order = 50
       });
   }
   ```
6. Add feature flag to `appsettings.json`

### Available Icons
`grid`, `cog`, `user`, `home`, `chart`, `folder`, `star`

---

## API Endpoints

Controllers and models live within their feature areas.

### Creating a New API Controller

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daisy.Areas.YourFeature;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class YourFeatureController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<YourModel>> GetAll() => Ok(items);

    [HttpGet("{id:int}")]
    public ActionResult<YourModel> GetById(int id)
    {
        var item = items.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return NotFound(new { error = $"Item with ID {id} not found" });
        return Ok(item);
    }

    [HttpPost]
    public ActionResult<YourModel> Create([FromBody] CreateRequest request)
    {
        // Create logic
        return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
    }

    [HttpPut("{id:int}")]
    public ActionResult<YourModel> Update(int id, [FromBody] UpdateRequest request)
    {
        // Update logic
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        // Delete logic
        return NoContent();
    }

    [HttpGet("protected")]
    [Authorize]
    public ActionResult<object> ProtectedEndpoint()
    {
        return Ok(new { message = $"Hello, {User.Identity?.Name}!" });
    }
}

public class YourModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

Routes map to `/api/[controller]`

---

## Theme System

### Configuration

```json
"Template": {
  "DefaultTheme": "dark"
}
```

**Available:** `light`, `dark`

### Persistence

Themes persist in `localStorage` under `daisy-theme`. Applied:
- Immediately on page load (prevents flash)
- After Blazor enhanced navigation

### Adding Themes

1. Update `tailwind.config.js`:
   ```javascript
   daisyui: {
     themes: ['light', 'dark', 'cupcake', 'forest'],
   }
   ```
2. Update theme switcher in `TopBar.razor`

### CSS-Based State

Data attributes coordinate CSS with JS:
```css
html[data-theme="dark"] { ... }
html[data-nav-collapsed="true"] .nav-sidebar { width: 4rem; }
```

---

## Component Patterns

### JavaScript Interop

Inline scripts in `App.razor` prevent FOUC:
```javascript
window.themeManager   // Theme persistence
window.navManager     // Nav collapse persistence
```

**Interop Guidelines:**
- Use `JS.InvokeAsync<T>()` for return values
- Wrap in try-catch for prerendering safety
- Minimize calls (prefer C# where possible)

### Lifecycle Methods

```csharp
protected override void OnInitialized()
{
    // Subscribe to events, sync data only
}

protected override async Task OnInitializedAsync()
{
    // Async data loading
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // JS interop (only works after render)
    }
}
```

### StateHasChanged Guidelines

- Call only when state actually changes
- Use `InvokeAsync(StateHasChanged)` in event handlers
- Prefer conditional: `if (newValue != oldValue)`

### When to Use InteractiveServer

Only use `@rendermode InteractiveServer` if component needs:
- Real-time updates
- Complex user interaction
- SignalR communication

Otherwise, prefer static SSR.

---

## Project Structure

```
src/Daisy/
├── Areas/                       # Feature areas (self-contained)
│   ├── Dashboard/Views/Pages/   # Dashboard pages
│   ├── Settings/Views/Pages/    # Settings pages
│   └── Example/                 # Template area
│       ├── ExampleController.cs # API controller + models
│       └── Views/Pages/         # Example pages
├── Components/                  # Shared Blazor components
│   ├── Layout/                  # MainLayout, TopBar, NavMenu
│   └── Pages/                   # Login, Error, NotFound
├── Controllers/                 # Redirect controllers (HomeController)
├── Infrastructure/              # Cross-cutting concerns
│   ├── Logging/                 # NLog configuration
│   └── Middleware/              # HTTP middleware
├── Modules/                     # App configuration (DaisyAppRegistry)
├── Services/                    # Business services
│   ├── Auth/                    # Authentication
│   └── Navigation/              # Menu management
├── Styles/                      # CSS source (Tailwind input)
├── wwwroot/                     # Static files
│   └── css/                     # Compiled CSS
├── appsettings.json             # Configuration
├── GlobalUsings.cs              # Explicit global usings
├── Program.cs                   # Application entry point
└── TEMPLATE.md                  # This file
```

---

## Namespace Renaming

### Manual Steps

1. **Solution file** (`Daisy.sln`) - Rename, update references
2. **Project file** (`Daisy.csproj`) - Rename, update `<RootNamespace>`
3. **All `.cs` files** - Find/replace `namespace Daisy` → `namespace YourProject`
4. **All `.razor` files** - Find/replace `@namespace Daisy` → `@namespace YourProject`
5. **Folder name** - Rename `src/Daisy` → `src/YourProject`
6. **Configuration** - Update cookie name and app name in `appsettings.json`

### IDE Refactoring

Most IDEs (Visual Studio, Rider) support namespace refactoring:
1. Right-click namespace → Rename/Refactor
2. Apply to all files

---

## Cleanup

After setup is complete, delete:
- [ ] `TEMPLATE.md` (this file)
- [ ] `Areas/Example/` (if not needed)
