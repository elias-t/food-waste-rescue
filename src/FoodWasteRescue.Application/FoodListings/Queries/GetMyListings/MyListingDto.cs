using FoodWasteRescue.Domain.Enums;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetMyListings;

public class MyListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public FoodCategory Category { get; set; }
    public ListingStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int ClaimCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
