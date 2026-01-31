#pragma warning disable IDE0130
namespace Deneblab.BlazorDaisy.Auth;

/// <summary>
/// Authentication modes supported by the template.
/// Configure via appsettings.json: Authentication.Mode
/// </summary>
public enum AuthMode
{
    /// <summary>
    /// No authentication - all pages are public.
    /// Use for public-facing sites or internal tools.
    /// </summary>
    None,

    /// <summary>
    /// Cookie-based authentication with built-in login page.
    /// Requires implementing your own user validation logic.
    /// </summary>
    Cookie,

    /// <summary>
    /// OAuth 2.0 authentication via external provider (Auth0, etc.).
    /// Requires configuring Auth0 section in appsettings.json.
    /// </summary>
    OAuth
}
