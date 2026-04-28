using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Commands.ClaimFoodListing;

public class ClaimFoodListingCommandHandler : IRequestHandler<ClaimFoodListingCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ClaimFoodListingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(ClaimFoodListingCommand request, CancellationToken cancellationToken)
    {
        var claimerId = _currentUserService.UserId;
        if (claimerId is null)
            return Result<Guid>.Failure("User not authenticated");

        var listing = await _context.FoodListings
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

        _context.Claims.Add(claim);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(claim.Id);
    }
}
