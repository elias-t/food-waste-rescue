using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Commands.CancelListing;

public class CancelListingCommandHandler : IRequestHandler<CancelListingCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CancelListingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CancelListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await _context.FoodListings
            .FirstOrDefaultAsync(l => l.Id == request.ListingId, cancellationToken);

        if (listing is null)
            return Result.Failure("Listing not found");

        if (listing.DonorId != _currentUserService.UserId)
            return Result.Failure("You are not authorised to cancel this listing");

        listing.Cancel();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
