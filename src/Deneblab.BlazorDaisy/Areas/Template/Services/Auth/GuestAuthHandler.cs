using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Deneblab.BlazorDaisy.Services.Auth;

/// <summary>
/// Handles guest authentication for development purposes.
/// Creates a temporary authenticated session without requiring OAuth.
/// </summary>
public static class GuestAuthHandler
{
    /// <summary>
    /// Creates a guest authentication session.
    /// Should only be enabled in development environment.
    /// </summary>
    public static async Task SignInAsGuestAsync(HttpContext context, IConfiguration config)
    {
        var guestEmail = config["Authentication:GuestEmail"] ?? "guest@example.com";
        var guestName = config["Authentication:GuestName"] ?? "Guest User";
        var guestId = "guest-user";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, guestId),
            new(ClaimTypes.Name, guestName),
            new(ClaimTypes.Email, guestEmail),
            new("sub", guestId),
            new("name", guestName),
            new("email", guestEmail),
            new("auth_method", "guest")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            AllowRefresh = true
        };

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);
    }

    /// <summary>
    /// Maps the guest login endpoint.
    /// Only available when AllowGuestMode is true in configuration.
    /// </summary>
    public static void MapGuestLogin(WebApplication app)
    {
        app.MapGet("/dev/guest-login", async (HttpContext context, IConfiguration config) =>
        {
            var allowGuest = config.GetValue<bool>("Authentication:AllowGuestMode");
            if (!allowGuest)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await SignInAsGuestAsync(context, config);
            context.Response.Redirect("/dashboard");
        });
    }
}
