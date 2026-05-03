using FoodWasteRescue.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Worker.Jobs;

public class ClaimConfirmationJob(
    IApplicationDbContext context,
    IEmailService emailService) : IClaimConfirmationJob
{
    public async Task SendAsync(Guid claimId)
    {
        var claim = await context.Claims
            .Include(c => c.Listing)
                .ThenInclude(l => l.Donor)
            .Include(c => c.Claimer)
            .FirstOrDefaultAsync(c => c.Id == claimId);

        if (claim is null) return;

        // Email to claimer
        await emailService.SendAsync(
            claim.Claimer.Email!,
            "Claim Confirmed!",
            $"You've claimed {claim.Listing.Title}. " +
            $"Please collect before {claim.Listing.ExpiresAt:HH:mm} " +
            $"at {claim.Listing.Address}.");

        // Email to donor
        await emailService.SendAsync(
            claim.Listing.Donor.Email!,
            "Your food has been claimed!",
            $"{claim.Claimer.DisplayName} will collect " +
            $"{claim.Listing.Title} before " +
            $"{claim.Listing.ExpiresAt:HH:mm}.");
    }
}
