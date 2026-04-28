using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Worker.Jobs;

public class ClaimReminderJob(
    IApplicationDbContext context,
    IEmailService emailService) : IClaimReminderJob
{
    public async Task SendReminderAsync(Guid claimId)
    {
        var claim = await context.Claims
            .Include(c => c.Listing)
            .Include(c => c.Claimer)
            .FirstOrDefaultAsync(c => c.Id == claimId);

        if (claim is null) return;
        if (claim.Status != ClaimStatus.Confirmed) return;

        await emailService.SendAsync(
            claim.Claimer.Email!,
            "Reminder: collect your food soon!",
            $"Your collection of {claim.Listing.Title} " +
            $"expires at {claim.Listing.ExpiresAt:HH:mm}. " +
            $"Don't forget to pick it up at {claim.Listing.Address}.");
    }
}
