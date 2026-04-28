namespace FoodWasteRescue.Application.FoodListings.Queries.GetUnclaimedExpiringSoon;

public record UnclaimedListingDto(
    Guid Id,
    string Title,
    DateTime ExpiresAt,
    double? Latitude,
    double? Longitude,
    string? Address);
