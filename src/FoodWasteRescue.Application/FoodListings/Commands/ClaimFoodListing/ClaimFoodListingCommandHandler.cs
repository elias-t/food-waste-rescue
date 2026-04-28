using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Commands.ClaimFoodListing;

public class ClaimFoodListingCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient)
    : IRequestHandler<ClaimFoodListingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ClaimFoodListingCommand request, CancellationToken cancellationToken)
    {
        var claimerId = currentUserService.UserId;
        if (claimerId is null)
            return Result<Guid>.Failure("User not authenticated");

        var listing = await context.FoodListings
            .FirstOrDefaultAsync(l => l.Id == request.ListingId, cancellationToken);

        if (listing is null)
            return Result<Guid>.Failure("Listing not found");

        if (listing.Status != ListingStatus.Active)
            return Result<Guid>.Failure("Listing is not available for claiming");

        if (listing.DonorId == claimerId)
            return Result<Guid>.Failure("You cannot claim your own listing");

        var claim = new Claim(request.ListingId, claimerId, request.Notes);
        claim.Confirm();
        listing.MarkAsClaimed();

        context.Claims.Add(claim);
        await context.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<IClaimConfirmationJob>(
            j => j.SendAsync(claim.Id));

        backgroundJobClient.Schedule<IClaimReminderJob>(
            j => j.SendReminderAsync(claim.Id),
            listing.ExpiresAt.AddHours(-1));

        return Result<Guid>.Success(claim.Id);
    }
}
