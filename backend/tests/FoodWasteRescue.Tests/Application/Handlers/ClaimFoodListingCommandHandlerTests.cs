using FluentAssertions;
using FoodWasteRescue.Application.FoodListings.Commands.ClaimFoodListing;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Tests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using NSubstitute;

namespace FoodWasteRescue.Tests.Application.Handlers;

public class ClaimFoodListingCommandHandlerTests : IDisposable
{
    private readonly DbContextFactory _factory = new();
    private readonly FakeCurrentUser _currentUser = new();
    private readonly IBackgroundJobClient _jobClient = Substitute.For<IBackgroundJobClient>();
    private readonly ClaimFoodListingCommandHandler _sut;

    public ClaimFoodListingCommandHandlerTests()
    {
        _sut = new ClaimFoodListingCommandHandler(_factory.Context, _currentUser, _jobClient);
    }

    private async Task<(string donorId, string claimerId, FoodListing listing)> SeedAsync()
    {
        var donor = await _factory.SeedUserAsync("donor-1", "donor@test.com", UserRole.Donor);
        var claimer = await _factory.SeedUserAsync("claimer-1", "claimer@test.com", UserRole.Claimer);
        var listing = new FoodListing(
            "Bread", "Fresh bread", FoodCategory.Bakery, "5 loaves",
            DateTime.UtcNow.AddDays(1), donor.Id, 51.5, -0.1, null);
        _factory.Context.FoodListings.Add(listing);
        await _factory.Context.SaveChangesAsync(CancellationToken.None);
        return (donor.Id, claimer.Id, listing);
    }

    [Fact]
    public async Task Handle_ValidClaim_CreatesClaimAndMarksListingClaimed()
    {
        var (_, claimerId, listing) = await SeedAsync();
        _currentUser.UserId = claimerId;

        var result = await _sut.Handle(new ClaimFoodListingCommand(listing.Id, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _factory.Context.Claims.Should().ContainSingle(c => c.Id == result.Value);
        _factory.Context.FoodListings.Find(listing.Id)!.Status.Should().Be(ListingStatus.Claimed);
        _jobClient.Received(2).Create(Arg.Any<Job>(), Arg.Any<IState>());
    }

    [Fact]
    public async Task Handle_AlreadyClaimedListing_ReturnsFailure()
    {
        var (_, claimerId, listing) = await SeedAsync();
        listing.MarkAsClaimed();
        await _factory.Context.SaveChangesAsync(CancellationToken.None);
        _currentUser.UserId = claimerId;

        var result = await _sut.Handle(new ClaimFoodListingCommand(listing.Id, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_OwnListing_ReturnsFailure()
    {
        var (donorId, _, listing) = await SeedAsync();
        _currentUser.UserId = donorId;

        var result = await _sut.Handle(new ClaimFoodListingCommand(listing.Id, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Unauthenticated_ReturnsFailure()
    {
        var (_, _, listing) = await SeedAsync();
        _currentUser.UserId = null;

        var result = await _sut.Handle(new ClaimFoodListingCommand(listing.Id, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    public void Dispose() => _factory.Dispose();
}
