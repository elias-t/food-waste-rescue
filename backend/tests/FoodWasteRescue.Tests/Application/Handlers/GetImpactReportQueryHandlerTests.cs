using FluentAssertions;
using FoodWasteRescue.Application.FoodListings.Queries.GetImpactReport;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Tests.Helpers;

namespace FoodWasteRescue.Tests.Application.Handlers;

public class GetImpactReportQueryHandlerTests : IDisposable
{
    private readonly DbContextFactory _factory = new();
    private readonly GetImpactReportQueryHandler _sut;

    public GetImpactReportQueryHandlerTests()
    {
        _sut = new GetImpactReportQueryHandler(_factory.Context);
    }

    [Fact]
    public async Task Handle_EmptyDb_ReturnsZeroCounts()
    {
        var result = await _sut.Handle(new GetImpactReportQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalListings.Should().Be(0);
        result.Value.TotalClaimed.Should().Be(0);
        result.Value.TotalExpired.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithMixedStatuses_ReturnsCorrectCounts()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        var active = new FoodListing("Active", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id);
        var claimed = new FoodListing("Claimed", "desc", FoodCategory.Dairy, "1", DateTime.UtcNow.AddDays(1), donor.Id);
        claimed.MarkAsClaimed();
        var expired = new FoodListing("Expired", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id);
        expired.Expire();
        _factory.Context.FoodListings.AddRange(active, claimed, expired);
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        var result = await _sut.Handle(new GetImpactReportQuery(), CancellationToken.None);

        result.Value!.TotalListings.Should().Be(3);
        result.Value.TotalClaimed.Should().Be(1);
        result.Value.TotalExpired.Should().Be(1);
    }

    [Fact]
    public async Task Handle_FoodSavedByCategory_OnlyCountsClaimedListings()
    {
        var donor = await _factory.SeedUserAsync("donor-1");
        var claimed = new FoodListing("C1", "desc", FoodCategory.Bakery, "1", DateTime.UtcNow.AddDays(1), donor.Id);
        claimed.MarkAsClaimed();
        var active = new FoodListing("A1", "desc", FoodCategory.Dairy, "1", DateTime.UtcNow.AddDays(1), donor.Id);
        _factory.Context.FoodListings.AddRange(claimed, active);
        await _factory.Context.SaveChangesAsync(CancellationToken.None);

        var result = await _sut.Handle(new GetImpactReportQuery(), CancellationToken.None);

        result.Value!.FoodSavedByCategory.Should().ContainKey("Bakery").WhoseValue.Should().Be(1);
        result.Value.FoodSavedByCategory.Should().NotContainKey("Dairy");
    }

    public void Dispose() => _factory.Dispose();
}
