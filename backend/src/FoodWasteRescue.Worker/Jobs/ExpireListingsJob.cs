using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Worker.Jobs;

public class ExpireListingsJob(IApplicationDbContext db, ILogger<ExpireListingsJob> logger) : IExpireListingsJob
{
    public async Task ExecuteAsync()
    {
        var now = DateTime.UtcNow;

        var listings = await db.FoodListings
            .Where(l => l.Status == ListingStatus.Active && l.ExpiresAt <= now)
            .ToListAsync(CancellationToken.None);

        if (listings.Count == 0)
            return;

        foreach (var listing in listings)
            listing.Expire();

        await db.SaveChangesAsync(CancellationToken.None);

        logger.LogInformation("Expired {Count} food listings", listings.Count);
    }
}
