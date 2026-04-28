using FoodWasteRescue.Domain.Common;

namespace FoodWasteRescue.Domain.Events;

public class FoodListingCreatedEvent : BaseEvent
{
    public Guid ListingId { get; }

    public FoodListingCreatedEvent(Guid listingId) => ListingId = listingId;
}
