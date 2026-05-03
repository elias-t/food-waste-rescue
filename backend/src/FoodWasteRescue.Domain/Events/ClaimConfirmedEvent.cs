using FoodWasteRescue.Domain.Common;

namespace FoodWasteRescue.Domain.Events;

public class ClaimConfirmedEvent : BaseEvent
{
    public Guid ClaimId { get; }
    public Guid ListingId { get; }
    public string ClaimerId { get; }

    public ClaimConfirmedEvent(Guid claimId, Guid listingId, string claimerId)
    {
        ClaimId = claimId;
        ListingId = listingId;
        ClaimerId = claimerId;
    }
}
