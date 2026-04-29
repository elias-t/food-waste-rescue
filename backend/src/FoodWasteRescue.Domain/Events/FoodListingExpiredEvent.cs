using FoodWasteRescue.Domain.Common;

namespace FoodWasteRescue.Domain.Events;

public class FoodListingExpiredEvent : BaseEvent
{
    public Guid ListingId { get; }

    public FoodListingExpiredEvent(Guid listingId) => ListingId = listingId;
}
