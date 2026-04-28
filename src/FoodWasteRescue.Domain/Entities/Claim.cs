using FoodWasteRescue.Domain.Common;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Domain.Events;

namespace FoodWasteRescue.Domain.Entities;

public class Claim : BaseEntity
{
    public Guid ListingId { get; private set; }
    public string ClaimerId { get; private set; } = string.Empty;
    public ClaimStatus Status { get; private set; } = ClaimStatus.Pending;
    public string? Notes { get; private set; }
    public DateTime? CollectedAt { get; private set; }

    public FoodListing Listing { get; private set; } = null!;
    public ApplicationUser Claimer { get; private set; } = null!;

    protected Claim() { }

    public Claim(Guid listingId, string claimerId, string? notes = null)
    {
        ListingId = listingId;
        ClaimerId = claimerId;
        Notes = notes;
    }

    public void Confirm()
    {
        Status = ClaimStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ClaimConfirmedEvent(Id, ListingId, ClaimerId));
    }

    public void MarkCollected()
    {
        Status = ClaimStatus.Collected;
        CollectedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ClaimStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
