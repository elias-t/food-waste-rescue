using FluentAssertions;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Domain.Events;

namespace FoodWasteRescue.Tests.Domain;

public class FoodListingTests
{
    private static FoodListing Make() => new(
        "Test Listing", "Description", FoodCategory.Bakery,
        "10 loaves", DateTime.UtcNow.AddDays(1), "donor-1");

    [Fact]
    public void Constructor_SetsStatusToActive()
    {
        Make().Status.Should().Be(ListingStatus.Active);
    }

    [Fact]
    public void Constructor_FiresFoodListingCreatedEvent()
    {
        var listing = Make();
        listing.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<FoodListingCreatedEvent>();
    }

    [Fact]
    public void Expire_SetsStatusToExpired()
    {
        var listing = Make();
        listing.Expire();
        listing.Status.Should().Be(ListingStatus.Expired);
    }

    [Fact]
    public void Expire_FiresFoodListingExpiredEvent()
    {
        var listing = Make();
        listing.ClearDomainEvents();
        listing.Expire();
        listing.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<FoodListingExpiredEvent>();
    }

    [Fact]
    public void Cancel_SetsStatusToCancelled()
    {
        var listing = Make();
        listing.Cancel();
        listing.Status.Should().Be(ListingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_FiresNoDomainEvents()
    {
        var listing = Make();
        listing.ClearDomainEvents();
        listing.Cancel();
        listing.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void MarkAsClaimed_SetsStatusToClaimed()
    {
        var listing = Make();
        listing.MarkAsClaimed();
        listing.Status.Should().Be(ListingStatus.Claimed);
    }
}
