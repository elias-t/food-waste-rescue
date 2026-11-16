using FluentAssertions;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Domain.Events;

namespace FoodWasteRescue.Tests.Domain;

public class ClaimTests
{
    private static Claim Make() => new(Guid.NewGuid(), "claimer-1", null);

    [Fact]
    public void Constructor_SetsStatusToPending()
    {
        Make().Status.Should().Be(ClaimStatus.Pending);
    }

    [Fact]
    public void Confirm_SetsStatusToConfirmed()
    {
        var claim = Make();
        claim.Confirm();
        claim.Status.Should().Be(ClaimStatus.Confirmed);
    }

    [Fact]
    public void Confirm_FiresClaimConfirmedEvent()
    {
        var claim = Make();
        claim.ClearDomainEvents();
        claim.Confirm();
        claim.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ClaimConfirmedEvent>();
    }

    [Fact]
    public void MarkCollected_SetsStatusToCollected()
    {
        var claim = Make();
        claim.MarkCollected();
        claim.Status.Should().Be(ClaimStatus.Collected);
    }

    [Fact]
    public void MarkCollected_SetsCollectedAt()
    {
        var before = DateTime.UtcNow;
        var claim = Make();
        claim.MarkCollected();
        claim.CollectedAt.Should().NotBeNull().And.BeOnOrAfter(before);
    }

    [Fact]
    public void Cancel_SetsStatusToCancelled()
    {
        var claim = Make();
        claim.Cancel();
        claim.Status.Should().Be(ClaimStatus.Cancelled);
    }
}
