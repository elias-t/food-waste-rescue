using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Commands.ExpireListings;

public class ExpireListingsCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ExpireListingsCommand>
{
    public async Task Handle(ExpireListingsCommand request, CancellationToken cancellationToken)
    {
        var expiredListings = await context.FoodListings
            .Where(x => x.Status == ListingStatus.Active && x.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var listing in expiredListings)
        {
            listing.Expire();
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
