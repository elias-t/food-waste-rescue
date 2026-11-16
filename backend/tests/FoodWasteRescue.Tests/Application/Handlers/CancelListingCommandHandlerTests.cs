using FluentAssertions;
using FoodWasteRescue.Application.FoodListings.Commands.CancelListing;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Tests.Helpers;

namespace FoodWasteRescue.Tests.Application.Handlers;

public class CancelListingCommandHandlerTests : IDisposable
{
    private readonly DbContextFactory _factory = new();
    private readonly FakeCurrentUser _currentUser = new();
    private readonly CancelListingCommandHandler _sut;

    public CancelListingCommandHandlerTests()
    {
        _sut = new CancelListingCommandHandler(_factory.Context, _currentUser);
    }

    private async Task<FoodListing> SeedListingAsync(string donorId)
    {
        await _factory.SeedUserAsync(donorId, $"{donorId}@test.com");
        var listing = new FoodListing("Bread", "Fresh bread", FoodCategory.Bakery, "5 loaves", DateTime.UtcNow.AddDays(1), donorId);
        _factory.Context.FoodListings.Add(listing);
        await _factory.Context.SaveChangesAsync(CancellationToken.None);
        return listing;
    }

    [Fact]
    public async Task Handle_OwnListing_CancelsSuccessfully()
    {
        var listing = await SeedListingAsync("donor-1");
        _currentUser.UserId = "donor-1";

        var result = await _sut.Handle(new CancelListingCommand(listing.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _factory.Context.FoodListings.Find(listing.Id)!.Status.Should().Be(ListingStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_OtherUserListing_ReturnsFailure()
    {
        var listing = await SeedListingAsync("donor-1");
        _currentUser.UserId = "other-user";

        var result = await _sut.Handle(new CancelListingCommand(listing.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NonexistentListing_ReturnsFailure()
    {
        _currentUser.UserId = "donor-1";

        var result = await _sut.Handle(new CancelListingCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    public void Dispose() => _factory.Dispose();
}
