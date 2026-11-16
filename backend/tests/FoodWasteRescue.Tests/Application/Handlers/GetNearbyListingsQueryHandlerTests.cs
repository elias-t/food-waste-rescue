using FluentAssertions;
using FoodWasteRescue.Application.FoodListings.Queries.GetNearbyListings;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Tests.Helpers;

namespace FoodWasteRescue.Tests.Application.Handlers;

public class GetNearbyListingsQueryHandlerTests : IDisposable
{
    private readonly DbContextFactory _factory = new();
    private readonly GetNearbyListingsQueryHandler _sut;

    public GetNearbyListingsQueryHandlerTests()
    {
        _sut = new GetNearbyListingsQueryHandler(_factory.Context);
    }

    // Query point: 51.5, -0.1 (London-ish)
    // Near: 51.51, -0.1 ≈ 1.1 km away
    // Far:  51.60, -0.1 ≈ 11 km away

    private async Task SeedAsync()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        _factory.Context.FoodListings.AddRange(
            new FoodListing("Near", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id, 51.51, -0.1),
            new FoodListing("Far", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id, 51.60, -0.1),
            new FoodListing("No Coords", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id));
        await _factory.Context.SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ReturnsListingsWithinRadius()
    {
        await SeedAsync();
        var result = await _sut.Handle(new GetNearbyListingsQuery(51.5, -0.1, 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle(l => l.Title == "Near");
    }

    [Fact]
    public async Task Handle_ExcludesListingsOutsideRadius()
    {
        await SeedAsync();
        var result = await _sut.Handle(new GetNearbyListingsQuery(51.5, -0.1, 10), CancellationToken.None);

        result.Value.Should().NotContain(l => l.Title == "Far");
    }

    [Fact]
    public async Task Handle_ExcludesListingsWithoutCoordinates()
    {
        await SeedAsync();
        var result = await _sut.Handle(new GetNearbyListingsQuery(51.5, -0.1, 50), CancellationToken.None);

        result.Value.Should().NotContain(l => l.Title == "No Coords");
    }

    [Fact]
    public async Task Handle_ResultsOrderedByDistanceAscending()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        _factory.Context.FoodListings.AddRange(
            new FoodListing("Medium", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id, 51.51, -0.1),
            new FoodListing("Close", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id, 51.501, -0.1));
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        var result = await _sut.Handle(new GetNearbyListingsQuery(51.5, -0.1, 10), CancellationToken.None);

        result.Value![0].Title.Should().Be("Close");
        result.Value![1].Title.Should().Be("Medium");
    }

    [Fact]
    public async Task Handle_RadiusCappedAt50km_ExcludesListingsBeyond50km()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        // ~67 km north of query point
        _factory.Context.FoodListings.Add(
            new FoodListing("Far Away", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id, 52.1, -0.1));
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        // Requesting 200 km should be capped to 50 km
        var result = await _sut.Handle(new GetNearbyListingsQuery(51.5, -0.1, 200), CancellationToken.None);

        result.Value.Should().BeEmpty();
    }

    public void Dispose() => _factory.Dispose();
}
