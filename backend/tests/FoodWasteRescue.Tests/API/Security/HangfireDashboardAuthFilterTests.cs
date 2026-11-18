using System.Net;
using System.Text;
using FoodWasteRescue.API.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace FoodWasteRescue.Tests.API.Security;

public class HangfireDashboardAuthFilterTests
{
    private const string Username = "admin";
    private const string Password = "correct-horse-battery-staple";

    [Fact]
    public void Denies_anonymous_request_in_production()
    {
        var filter = CreateFilter("Production", withCredentials: true);
        var http = CreateRequest(https: true);

        filter.IsAuthorized(http).Should().BeFalse();
        http.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        http.Response.Headers.WWWAuthenticate.ToString().Should().Contain("Basic");
    }

    [Fact]
    public void Allows_request_with_correct_credentials()
    {
        var filter = CreateFilter("Production", withCredentials: true);
        var http = CreateRequest(https: true, basicAuth: (Username, Password));

        filter.IsAuthorized(http).Should().BeTrue();
    }

    [Theory]
    [InlineData("admin", "wrong-password")]
    [InlineData("wrong-user", "correct-horse-battery-staple")]
    [InlineData("", "")]
    public void Denies_request_with_wrong_credentials(string username, string password)
    {
        var filter = CreateFilter("Production", withCredentials: true);
        var http = CreateRequest(https: true, basicAuth: (username, password));

        filter.IsAuthorized(http).Should().BeFalse();
        http.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public void Fails_closed_when_no_credentials_are_configured()
    {
        // The whole point of the fix: a misconfigured deployment must lock the dashboard,
        // not publish it. Correct-looking credentials must not get in either.
        var filter = CreateFilter("Production", withCredentials: false);
        var http = CreateRequest(https: true, basicAuth: (Username, Password));

        filter.IsAuthorized(http).Should().BeFalse();
        http.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        // No prompt: nothing the user could type would work.
        http.Response.Headers.WWWAuthenticate.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Denies_credentials_sent_over_plain_http()
    {
        var filter = CreateFilter("Production", withCredentials: true);
        var http = CreateRequest(https: false, basicAuth: (Username, Password));

        filter.IsAuthorized(http).Should().BeFalse();
        http.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public void Allows_local_request_in_development_without_credentials()
    {
        var filter = CreateFilter("Development", withCredentials: false);
        var http = CreateRequest(https: false, remoteIp: IPAddress.Loopback);

        filter.IsAuthorized(http).Should().BeTrue();
    }

    [Fact]
    public void Denies_remote_request_in_development()
    {
        var filter = CreateFilter("Development", withCredentials: false);
        var http = CreateRequest(https: true, remoteIp: IPAddress.Parse("203.0.113.7"));

        filter.IsAuthorized(http).Should().BeFalse();
    }

    [Fact]
    public void Denies_local_request_in_production_without_credentials()
    {
        // Loopback is not a free pass outside Development: on App Service a request
        // proxied in-process could otherwise bypass the check.
        var filter = CreateFilter("Production", withCredentials: false);
        var http = CreateRequest(https: true, remoteIp: IPAddress.Loopback);

        filter.IsAuthorized(http).Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Bearer some.jwt.token")]
    [InlineData("Basic")]
    [InlineData("Basic !!!not-base64!!!")]
    [InlineData("Basic bm8tY29sb24taGVyZQ==")]  // "no-colon-here"
    public void Denies_malformed_authorization_headers(string header)
    {
        var filter = CreateFilter("Production", withCredentials: true);
        var http = CreateRequest(https: true);
        if (header.Length > 0)
        {
            http.Request.Headers.Authorization = header;
        }

        filter.IsAuthorized(http).Should().BeFalse();
        http.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public void Accepts_password_containing_colons()
    {
        var filter = CreateFilter("Production", withCredentials: true, password: "a:b:c");
        var http = CreateRequest(https: true, basicAuth: (Username, "a:b:c"));

        filter.IsAuthorized(http).Should().BeTrue();
    }

    private static HangfireDashboardAuthFilter CreateFilter(
        string environmentName,
        bool withCredentials,
        string password = Password)
    {
        var settings = new Dictionary<string, string?>();
        if (withCredentials)
        {
            settings["Hangfire:DashboardAuth:Username"] = Username;
            settings["Hangfire:DashboardAuth:Password"] = password;
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var environment = new StubWebHostEnvironment { EnvironmentName = environmentName };

        return new HangfireDashboardAuthFilter(configuration, environment);
    }

    private static DefaultHttpContext CreateRequest(
        bool https,
        (string Username, string Password)? basicAuth = null,
        IPAddress? remoteIp = null)
    {
        var http = new DefaultHttpContext();
        http.Request.Scheme = https ? "https" : "http";
        http.Connection.RemoteIpAddress = remoteIp;

        if (basicAuth is { } credentials)
        {
            var encoded = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{credentials.Username}:{credentials.Password}"));
            http.Request.Headers.Authorization = $"Basic {encoded}";
        }

        return http;
    }

    private sealed class StubWebHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Production";
        public string ApplicationName { get; set; } = "FoodWasteRescue.API";
        public string ContentRootPath { get; set; } = string.Empty;
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
