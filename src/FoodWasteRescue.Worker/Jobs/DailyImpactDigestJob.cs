using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.FoodListings.Queries.GetDailyImpactDigest;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FoodWasteRescue.Worker.Jobs;

public class DailyImpactDigestJob(
    IMediator mediator,
    IEmailService emailService,
    IConfiguration configuration) : IDailyImpactDigestJob
{
    public async Task ExecuteAsync()
    {
        var result = await mediator.Send(new GetDailyImpactDigestQuery());
        if (!result.IsSuccess) return;

        var digest = result.Value!;
        var adminEmail = configuration["Admin:Email"] ?? "admin@foodwasterescue.local";

        var body = $"""
            Daily Impact Digest for {digest.Date:yyyy-MM-dd}

            New listings posted: {digest.NewListingsCount}
            Listings claimed:    {digest.ClaimedCount}
            Listings expired:    {digest.ExpiredCount}
            Top donor:           {digest.TopDonor ?? "N/A"}

            Keep up the great work!
            """;

        await emailService.SendAsync(adminEmail, "Daily Impact Digest", body);
    }
}
