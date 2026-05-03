using FoodWasteRescue.Domain.Enums;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetListingDetail;

public class ListingDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FoodCategory Category { get; set; }
    public string QuantityDescription { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public ListingStatus Status { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? DonorName { get; set; }
    public string? OrganisationName { get; set; }
    public int ClaimCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
