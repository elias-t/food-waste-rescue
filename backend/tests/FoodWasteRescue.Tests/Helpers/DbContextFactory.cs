using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Infrastructure.Persistence;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FoodWasteRescue.Tests.Helpers;

public sealed class DbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;
    public AppDbContext Context { get; }

    public DbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new AppDbContext(options, Substitute.For<IMediator>());
        Context.Database.EnsureCreated();
    }

    public async Task<ApplicationUser> SeedUserAsync(
        string userId = "user-1",
        string email = "test@test.com",
        UserRole role = UserRole.Donor)
    {
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = email,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            DisplayName = "Test User",
            Role = role,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        Context.Users.Add(user);
        await Context.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
