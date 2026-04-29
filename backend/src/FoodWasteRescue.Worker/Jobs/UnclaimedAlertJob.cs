using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.FoodListings.Queries.GetUnclaimedExpiringSoon;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FoodWasteRescue.Worker.Jobs;

public class UnclaimedAlertJob(
    IMediator mediator,
    ILogger<UnclaimedAlertJob> logger) : IUnclaimedAlertJob
{
    public async Task ExecuteAsync()
    {
        var result = await mediator.Send(new GetUnclaimedExpiringSoonQuery());
        if (!result.IsSuccess) return;

        var listings = result.Value!;
        if (listings.Count == 0)
        {
            logger.LogInformation("No unclaimed listings expiring within 3 hours");
            return;
        }

        logger.LogWarning(
            "ALERT: {Count} unclaimed listings expiring within 3 hours",
            listings.Count);

        foreach (var listing in listings)
        {
            logger.LogWarning(
                " -> {Title} expires at {ExpiresAt} | Address: {Address}",
                listing.Title, listing.ExpiresAt, listing.Address);
        }
    }
}
