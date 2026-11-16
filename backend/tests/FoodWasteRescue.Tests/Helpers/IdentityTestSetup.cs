using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Infrastructure.Persistence;
using FoodWasteRescue.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FoodWasteRescue.Tests.Helpers;

public sealed class IdentityTestSetup : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;

    public IIdentityService IdentityService { get; }

    public IdentityTestSetup()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddSingleton<IMediator>(Substitute.For<IMediator>());

        services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));

        services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireDigit = false;
                o.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "super-secret-test-key-minimum-32-chars!!",
                ["JwtSettings:Issuer"] = "test-issuer",
                ["JwtSettings:Audience"] = "test-audience",
                ["JwtSettings:ExpiryDays"] = "7"
            })
            .Build();
        services.AddSingleton<IConfiguration>(config);
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IIdentityService, IdentityService>();

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();

        var db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();

        IdentityService = _scope.ServiceProvider.GetRequiredService<IIdentityService>();
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
        _connection.Dispose();
    }
}
