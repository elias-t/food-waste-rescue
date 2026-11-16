using FluentAssertions;
using FoodWasteRescue.Application.FoodListings.Queries.GetUnclaimedExpiringSoon;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Tests.Helpers;

namespace FoodWasteRescue.Tests.Application.Handlers;

public class GetUnclaimedExpiringSoonQueryHandlerTests : IDisposable
{
    private readonly DbContextFactory _factory = new();
    private readonly GetUnclaimedExpiringSoonQueryHandler _sut;

    public GetUnclaimedExpiringSoonQueryHandlerTests()
    {
        _sut = new GetUnclaimedExpiringSoonQueryHandler(_factory.Context);
    }

    [Fact]
    public async Task Handle_ReturnsActiveListingsExpiringWithin3Hours()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        _factory.Context.FoodListings.Add(new FoodListing(
            "Expiring Soon", "desc", FoodCategory.Bakery, "1",
            DateTime.UtcNow.AddHours(2), donor.Id));
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        var result = await _sut.Handle(new GetUnclaimedExpiringSoonQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle(l => l.Title == "Expiring Soon");
    }

    [Fact]
    public async Task Handle_ExcludesListingsExpiringAfter3Hours()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        _factory.Context.FoodListings.Add(new FoodListing(
            "Later", "desc", FoodCategory.Bakery, "1",
            DateTime.UtcNow.AddHours(4), donor.Id));
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        var result = await _sut.Handle(new GetUnclaimedExpiringSoonQuery(), CancellationToken.None);

        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ExcludesNonActiveListings()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        var listing = new FoodListing(
            "Claimed Expiring", "desc", FoodCategory.Bakery, "1",
            DateTime.UtcNow.AddHours(2), donor.Id);
        listing.MarkAsClaimed();
        _factory.Context.FoodListings.Add(listing);
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        var result = await _sut.Handle(new GetUnclaimedExpiringSoonQuery(), CancellationToken.None);

        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ExcludesListingsWithExistingClaims()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        var claimer = await _factory.SeedUserAsync("claimer-1", "claimer@test.com");
        var listing = new FoodListing(
            "Has Claim", "desc", FoodCategory.Bakery, "1",
            DateTime.UtcNow.AddHours(2), donor.Id);
        _factory.Context.FoodListings.Add(listing);
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        _factory.Context.Claims.Add(new Claim(listing.Id, claimer.Id, null));
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        var result = await _sut.Handle(new GetUnclaimedExpiringSoonQuery(), CancellationToken.None);

        result.Value.Should().BeEmpty();
    }

    public void Dispose() => _factory.Dispose();
}
