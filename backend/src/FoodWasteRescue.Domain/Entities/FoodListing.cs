using FoodWasteRescue.Domain.Common;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Domain.Events;

namespace FoodWasteRescue.Domain.Entities;

public class FoodListing : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public FoodCategory Category { get; private set; }
    public string QuantityDescription { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public ListingStatus Status { get; private set; } = ListingStatus.Active;
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public string? Address { get; private set; }
    public string DonorId { get; private set; } = string.Empty;

    public ApplicationUser Donor { get; private set; } = null!;
    public ICollection<Claim> Claims { get; private set; } = [];

    protected FoodListing() { }

    public FoodListing(string title, string? description, FoodCategory category,
        string quantityDescription, DateTime expiresAt, string donorId,
        double? latitude = null, double? longitude = null, string? address = null)
    {
        Title = title;
        Description = description;
        Category = category;
        QuantityDescription = quantityDescription;
        ExpiresAt = expiresAt;
        DonorId = donorId;
        Latitude = latitude;
        Longitude = longitude;
        Address = address;

        AddDomainEvent(new FoodListingCreatedEvent(Id));
    }

    public void Expire()
    {
        Status = ListingStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new FoodListingExpiredEvent(Id));
    }

    public void Cancel()
    {
        Status = ListingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsClaimed()
    {
        Status = ListingStatus.Claimed;
        UpdatedAt = DateTime.UtcNow;
    }
}
