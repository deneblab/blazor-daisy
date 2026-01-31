

using System;
using System.Collections.Generic;
using Deneblab.BlazorDaisy.Areas.Template.App;
using Deneblab.BlazorDaisy.Auth;
using Deneblab.BlazorDaisy.Components;
using Deneblab.BlazorDaisy.Components.Snackbar;
using Deneblab.BlazorDaisy.Infrastructure.Logging;
using Deneblab.BlazorDaisy.Infrastructure.Middleware;
using Deneblab.BlazorDaisy.Navigation;
using Deneblab.SimpleVersion;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Deneblab.BlazorDaisy;

public class Program
{
    public static void Main(string[] args)
    {

        var env = Deneblab.SimpleEnv.SimpleEnv.Detect();
        var configDir = env.ConfigDir;
        var version = SimpleVersionParser.FromCurrentApp();
        var config = DaisyAppConfig.Load(env);
        var reg = new DaisyAppRegistry(env, config, version);
        var sem = reg.Version.SemVer;
        
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddSingleton(reg);
        // Configure logging (NLog)
        LoggingSetup.Configure(builder, reg.Env.LogDir);

        // Add services
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Add cascading authentication state for Blazor
        builder.Services.AddCascadingAuthenticationState();

        // Add controllers for API endpoints in Areas
        builder.Services.AddControllers();

        // Navigation menu service (singleton for shared state)
        builder.Services.AddSingleton<IMenuService, MenuService>();

        // Snackbar notification service (scoped per circuit)
        builder.Services.AddScoped<ISnackbarService, SnackbarService>();

        // Configure authentication
        ConfigureAuthentication(builder);

        var app = builder.Build();

        // TEMPLATE: Register menu items based on enabled features
        // Configure features in appsettings.json under "Features" section
        RegisterDefaultMenuItems(
            app.Services.GetRequiredService<IMenuService>(),
            app.Services.GetRequiredService<IConfiguration>());

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Error handling middleware
        app.UseErrorHandling();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        // Map controllers (for API endpoints in Areas)
        app.MapControllers();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        // Auth endpoints
        MapAuthEndpoints(app);

        // Guest login endpoint (dev only)
        GuestAuthHandler.MapGuestLogin(app);

        app.Run();
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        // TEMPLATE: Authentication mode - configure in appsettings.json under "Authentication.Mode"
        // Options: "None", "Cookie", "OAuth"
        var authMode = AuthConfiguration.GetAuthMode(builder.Configuration);

        // Log any configuration warnings
        var warnings = AuthConfiguration.ValidateConfiguration(builder.Configuration);
        foreach (var warning in warnings)
        {
            Console.WriteLine($"[Auth Warning] {warning}");
        }

        // No authentication mode - skip auth setup entirely
        if (authMode == AuthMode.None)
        {
            return;
        }

        var auth0 = builder.Configuration.GetSection("Auth0");
        var domain = auth0["Domain"];
        var clientId = auth0["ClientId"];
        var hasOAuth = authMode == AuthMode.OAuth && AuthConfiguration.HasOAuthConfiguration(builder.Configuration);

        var authBuilder = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // Only use OIDC as challenge if configured, otherwise redirect to login page
            options.DefaultChallengeScheme = hasOAuth
                ? OpenIdConnectDefaults.AuthenticationScheme
                : CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            // TEMPLATE: Cookie settings - customize in appsettings.json under "Template" section
            var template = builder.Configuration.GetSection("Template");
            var cookieName = template["CookieName"] ?? "app-auth";
            var expirationDays = int.TryParse(template["CookieExpirationDays"], out var days) ? days : 7;
            var loginPath = template["LoginPath"] ?? "/login";
            var accessDeniedPath = template["AccessDeniedPath"] ?? "/access-denied";

            options.Cookie.Name = cookieName;
            options.ExpireTimeSpan = TimeSpan.FromDays(expirationDays);
            options.Cookie.MaxAge = TimeSpan.FromDays(expirationDays);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.LoginPath = loginPath;
            options.AccessDeniedPath = accessDeniedPath;
        });

        // Only add OpenIdConnect if Auth0 is configured
        if (hasOAuth)
        {
            authBuilder.AddOpenIdConnect(options =>
            {
                options.Authority = $"https://{domain}";
                options.ClientId = clientId;
                options.ClientSecret = auth0["ClientSecret"];
                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.CallbackPath = auth0["CallbackPath"] ?? "/auth/callback";
                options.SignedOutCallbackPath = auth0["SignedOutCallbackPath"] ?? "/auth/signout-callback";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });
        }

        builder.Services.AddAuthorization();
    }

    private static void MapAuthEndpoints(WebApplication app)
    {
        var config = app.Services.GetRequiredService<IConfiguration>();
        var authMode = AuthConfiguration.GetAuthMode(config);

        // No auth endpoints needed if authentication is disabled
        if (authMode == AuthMode.None)
        {
            return;
        }

        var hasOAuth = authMode == AuthMode.OAuth && AuthConfiguration.HasOAuthConfiguration(config);

        // TEMPLATE: Redirect paths - customize in appsettings.json under "Template" section
        var defaultLoginRedirect = config["Template:DefaultRedirectAfterLogin"] ?? "/dashboard";
        var defaultLogoutRedirect = config["Template:DefaultRedirectAfterLogout"] ?? "/";
        var loginPath = config["Template:LoginPath"] ?? "/login";

        app.MapGet("/auth/login", async (HttpContext context, string? returnUrl) =>
        {
            if (!hasOAuth)
            {
                // No OAuth configured - redirect to login page
                context.Response.Redirect(loginPath);
                return;
            }

            var redirectUri = returnUrl ?? defaultLoginRedirect;
            await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        });

        app.MapGet("/auth/logout", async (HttpContext context) =>
        {
            var idToken = await context.GetTokenAsync("id_token");

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (hasOAuth && !string.IsNullOrEmpty(idToken))
            {
                // OAuth user - sign out from provider too
                await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
                {
                    RedirectUri = defaultLogoutRedirect
                });
            }
            else
            {
                // Guest user or no OAuth - just redirect
                context.Response.Redirect(defaultLogoutRedirect);
            }
        });

        app.MapGet("/auth/signout-callback", () => Results.Redirect("/"));
    }

    private static void RegisterDefaultMenuItems(IMenuService menuService, IConfiguration config)
    {
        // TEMPLATE: Feature flags - enable/disable areas in appsettings.json under "Features"
        var features = config.GetSection("Features");
        var dashboardEnabled = features.GetValue<bool>("Dashboard", true);
        var settingsEnabled = features.GetValue<bool>("Settings", true);

        var menuItems = new List<MenuItem>();

        // Dashboard area (if enabled)
        if (dashboardEnabled)
        {
            menuItems.Add(new MenuItem
            {
                Id = "dashboard",
                Title = "Dashboard",
                Href = "/dashboard",
                Icon = "grid",
                Order = 1,
                Area = "Dashboard"
            });
        }

        // Settings area (if enabled)
        if (settingsEnabled)
        {
            menuItems.AddRange([
                new MenuItem
                {
                    Id = "settings",
                    Title = "Settings",
                    Href = "/example/settings",
                    Icon = "cog",
                    Order = 100
                },
                new MenuItem
                {
                    Id = "settings-general",
                    Title = "General",
                    Href = "/example/settings/general",
                    ParentId = "settings",
                    Order = 1
                },
                new MenuItem
                {
                    Id = "settings-profile",
                    Title = "Profile",
                    Href = "/example/settings/profile",
                    ParentId = "settings",
                    Order = 2
                },
                new MenuItem
                {
                    Id = "settings-security",
                    Title = "Security",
                    Href = "/example/settings/security",
                    ParentId = "settings",
                    Order = 3
                }
            ]);
        }

        // Example area (for demos and testing)
        if (features.GetValue<bool>("Example", true))
        {
            menuItems.Add(new MenuItem
            {
                Id = "example",
                Title = "Example",
                Href = "/example",
                Icon = "star",
                Order = 50
            });
        }

        menuService.RegisterRange(menuItems);
    }
}
