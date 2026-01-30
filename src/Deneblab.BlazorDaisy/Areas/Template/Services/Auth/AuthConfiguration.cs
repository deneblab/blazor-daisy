namespace Deneblab.BlazorDaisy.Services.Auth;

/// <summary>
/// Helper class for reading and validating authentication configuration.
/// </summary>
public static class AuthConfiguration
{
    /// <summary>
    /// Gets the configured authentication mode from appsettings.json.
    /// Defaults to Cookie if not specified or invalid.
    /// </summary>
    public static AuthMode GetAuthMode(IConfiguration configuration)
    {
        var modeString = configuration["Authentication:Mode"];

        if (string.IsNullOrEmpty(modeString))
        {
            // Legacy check: if Auth0 is configured, use OAuth mode
            if (HasOAuthConfiguration(configuration))
            {
                return AuthMode.OAuth;
            }
            return AuthMode.Cookie;
        }

        if (Enum.TryParse<AuthMode>(modeString, ignoreCase: true, out var mode))
        {
            return mode;
        }

        // Invalid mode specified - default to Cookie
        return AuthMode.Cookie;
    }

    /// <summary>
    /// Checks if OAuth (Auth0) configuration is present and valid.
    /// </summary>
    public static bool HasOAuthConfiguration(IConfiguration configuration)
    {
        var domain = configuration["Auth0:Domain"];
        var clientId = configuration["Auth0:ClientId"];
        return !string.IsNullOrEmpty(domain) && !string.IsNullOrEmpty(clientId);
    }

    /// <summary>
    /// Validates the authentication configuration and returns any warnings.
    /// </summary>
    public static List<string> ValidateConfiguration(IConfiguration configuration)
    {
        var warnings = new List<string>();
        var mode = GetAuthMode(configuration);

        if (mode == AuthMode.OAuth && !HasOAuthConfiguration(configuration))
        {
            warnings.Add("Authentication mode is set to OAuth but Auth0 configuration is missing. Falling back to Cookie mode.");
        }

        if (mode == AuthMode.None)
        {
            warnings.Add("Authentication is disabled. All pages will be publicly accessible.");
        }

        return warnings;
    }
}
