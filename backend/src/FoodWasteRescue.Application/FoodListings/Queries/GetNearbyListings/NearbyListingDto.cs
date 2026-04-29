using FoodWasteRescue.Domain.Enums;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetNearbyListings;

public class NearbyListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public FoodCategory Category { get; set; }
    public string QuantityDescription { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? DonorName { get; set; }
    public double DistanceKm { get; set; }
}
