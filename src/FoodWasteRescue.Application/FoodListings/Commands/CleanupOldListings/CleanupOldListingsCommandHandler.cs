using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Commands.CleanupOldListings;

public class CleanupOldListingsCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CleanupOldListingsCommand>
{
    public async Task Handle(CleanupOldListingsCommand request, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(-90);

        var oldListings = await context.FoodListings
            .Where(x => (x.Status == ListingStatus.Expired || x.Status == ListingStatus.Cancelled)
                && x.CreatedAt < cutoff)
            .ToListAsync(cancellationToken);

        context.FoodListings.RemoveRange(oldListings);
        await context.SaveChangesAsync(cancellationToken);
    }
}
