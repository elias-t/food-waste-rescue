using System.Net;
using System.Security.Cryptography;
using System.Text;
using Hangfire.Dashboard;

namespace FoodWasteRescue.API.Security;

/// <summary>
/// Guards the Hangfire dashboard.
/// <para>
/// The API authenticates with JWT bearer tokens, which a browser loading /hangfire cannot
/// supply, so a role check against <c>HttpContext.User</c> would lock the dashboard out of a
/// browser entirely. It uses HTTP Basic credentials held in configuration instead
/// (App Service application settings in production, user secrets locally).
/// </para>
/// <para>
/// Fails closed: with no credentials configured, nobody gets in. The only exception is a
/// local request on a developer machine, where the dashboard stays open for convenience.
/// </para>
/// </summary>
public sealed class HangfireDashboardAuthFilter(
    IConfiguration configuration,
    IWebHostEnvironment environment) : IDashboardAuthorizationFilter
{
    internal const string UsernameKey = "Hangfire:DashboardAuth:Username";
    internal const string PasswordKey = "Hangfire:DashboardAuth:Password";

    private const string Realm = "Hangfire";

    public bool Authorize(DashboardContext context) => IsAuthorized(context.GetHttpContext());

    internal bool IsAuthorized(HttpContext http)
    {
        // Developer convenience, and only that: Development plus a request from this machine.
        if (environment.IsDevelopment() && IsLocalRequest(http))
        {
            return true;
        }

        // Basic credentials are base64, not encryption, so never accept them over plain HTTP.
        if (!http.Request.IsHttps)
        {
            return Deny(http, promptForCredentials: false);
        }

        var expectedUsername = configuration[UsernameKey];
        var expectedPassword = configuration[PasswordKey];

        // No credentials configured means the dashboard is unreachable, rather than public.
        // No prompt either: there is nothing the user could type that would work.
        if (string.IsNullOrEmpty(expectedUsername) || string.IsNullOrEmpty(expectedPassword))
        {
            return Deny(http, promptForCredentials: false);
        }

        if (!TryReadBasicCredentials(http.Request.Headers.Authorization, out var username, out var password))
        {
            return Deny(http, promptForCredentials: true);
        }

        if (!FixedTimeEquals(username, expectedUsername) || !FixedTimeEquals(password, expectedPassword))
        {
            return Deny(http, promptForCredentials: true);
        }

        return true;
    }

    private static bool IsLocalRequest(HttpContext http)
    {
        var remote = http.Connection.RemoteIpAddress;
        if (remote is null)
        {
            return false;
        }

        if (IPAddress.IsLoopback(remote))
        {
            return true;
        }

        var local = http.Connection.LocalIpAddress;
        return local is not null && remote.Equals(local);
    }

    private static bool TryReadBasicCredentials(string? authorizationHeader, out string username, out string password)
    {
        username = string.Empty;
        password = string.Empty;

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return false;
        }

        const string scheme = "Basic ";
        if (!authorizationHeader.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var encoded = authorizationHeader[scheme.Length..].Trim();
        if (encoded.Length == 0)
        {
            return false;
        }

        string decoded;
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
        }
        catch (FormatException)
        {
            return false;
        }

        // Only the first colon separates the pair; a password may contain further colons.
        var separator = decoded.IndexOf(':');
        if (separator <= 0)
        {
            return false;
        }

        username = decoded[..separator];
        password = decoded[(separator + 1)..];
        return true;
    }

    private static bool FixedTimeEquals(string actual, string expected) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(actual),
            Encoding.UTF8.GetBytes(expected));

    private static bool Deny(HttpContext http, bool promptForCredentials)
    {
        http.Response.StatusCode = StatusCodes.Status401Unauthorized;

        if (promptForCredentials)
        {
            http.Response.Headers.WWWAuthenticate = $"Basic realm=\"{Realm}\", charset=\"UTF-8\"";
        }

        return false;
    }
}
