using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetImpactReport;

public class GetImpactReportQueryHandler : IRequestHandler<GetImpactReportQuery, Result<ImpactReportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetImpactReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ImpactReportDto>> Handle(GetImpactReportQuery request, CancellationToken cancellationToken)
    {
        var listings = await _context.FoodListings
            .AsNoTracking()
            .Include(l => l.Donor)
            .ToListAsync(cancellationToken);

        var mostActiveDonor = listings
            .GroupBy(l => l.DonorId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.First().Donor?.DisplayName ?? g.Key)
            .FirstOrDefault();

        var foodSavedByCategory = listings
            .Where(l => l.Status == ListingStatus.Claimed)
            .GroupBy(l => l.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var dto = new ImpactReportDto
        {
            TotalListings = listings.Count,
            TotalClaimed = listings.Count(l => l.Status == ListingStatus.Claimed),
            TotalExpired = listings.Count(l => l.Status == ListingStatus.Expired),
            MostActiveDonor = mostActiveDonor,
            FoodSavedByCategory = foodSavedByCategory
        };

        return Result<ImpactReportDto>.Success(dto);
    }
}
