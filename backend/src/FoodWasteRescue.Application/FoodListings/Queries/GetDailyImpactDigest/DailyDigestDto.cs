namespace FoodWasteRescue.Application.FoodListings.Queries.GetDailyImpactDigest;

public record DailyDigestDto(
    DateTime Date,
    int NewListingsCount,
    int ClaimedCount,
    int ExpiredCount,
    string? TopDonor);
