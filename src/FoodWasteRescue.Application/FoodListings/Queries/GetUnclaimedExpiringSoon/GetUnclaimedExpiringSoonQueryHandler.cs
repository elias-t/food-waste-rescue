using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetUnclaimedExpiringSoon;

public class GetUnclaimedExpiringSoonQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetUnclaimedExpiringSoonQuery, Result<List<UnclaimedListingDto>>>
{
    public async Task<Result<List<UnclaimedListingDto>>> Handle(
        GetUnclaimedExpiringSoonQuery request,
        CancellationToken cancellationToken)
    {
        var threshold = DateTime.UtcNow.AddHours(3);

        var listings = await context.FoodListings
            .AsNoTracking()
            .Where(x => x.Status == ListingStatus.Active
                && x.ExpiresAt <= threshold
                && !x.Claims.Any())
            .Select(x => new UnclaimedListingDto(
                x.Id,
                x.Title,
                x.ExpiresAt,
                x.Latitude,
                x.Longitude,
                x.Address))
            .ToListAsync(cancellationToken);

        return Result<List<UnclaimedListingDto>>.Success(listings);
    }
}
