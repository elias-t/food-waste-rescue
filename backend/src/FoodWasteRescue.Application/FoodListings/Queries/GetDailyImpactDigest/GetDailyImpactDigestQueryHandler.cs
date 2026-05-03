using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetDailyImpactDigest;

public class GetDailyImpactDigestQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetDailyImpactDigestQuery, Result<DailyDigestDto>>
{
    public async Task<Result<DailyDigestDto>> Handle(
        GetDailyImpactDigestQuery request,
        CancellationToken cancellationToken)
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var today = DateTime.UtcNow.Date;

        var newCount = await context.FoodListings
            .AsNoTracking()
            .CountAsync(x => x.CreatedAt >= yesterday && x.CreatedAt < today,
                cancellationToken);

        var claimedCount = await context.Claims
            .AsNoTracking()
            .CountAsync(x => x.CreatedAt >= yesterday && x.CreatedAt < today,
                cancellationToken);

        var expiredCount = await context.FoodListings
            .AsNoTracking()
            .CountAsync(x => x.Status == ListingStatus.Expired
                && x.UpdatedAt >= yesterday && x.UpdatedAt < today,
                cancellationToken);

        var topDonor = await context.FoodListings
            .AsNoTracking()
            .Where(x => x.CreatedAt >= yesterday && x.CreatedAt < today)
            .GroupBy(x => x.Donor.DisplayName)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<DailyDigestDto>.Success(new DailyDigestDto(
            yesterday, newCount, claimedCount, expiredCount, topDonor));
    }
}
