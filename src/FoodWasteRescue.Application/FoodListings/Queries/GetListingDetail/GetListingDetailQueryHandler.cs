using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetListingDetail;

public class GetListingDetailQueryHandler : IRequestHandler<GetListingDetailQuery, Result<ListingDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetListingDetailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ListingDetailDto>> Handle(GetListingDetailQuery request, CancellationToken cancellationToken)
    {
        var dto = await _context.FoodListings
            .AsNoTracking()
            .Include(l => l.Donor)
            .Where(l => l.Id == request.ListingId)
            .Select(l => new ListingDetailDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                Category = l.Category,
                QuantityDescription = l.QuantityDescription,
                ExpiresAt = l.ExpiresAt,
                Status = l.Status,
                Address = l.Address,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                DonorName = l.Donor.DisplayName,
                OrganisationName = l.Donor.OrganisationName,
                ClaimCount = l.Claims.Count,
                CreatedAt = l.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return dto is null
            ? Result<ListingDetailDto>.Failure("Listing not found")
            : Result<ListingDetailDto>.Success(dto);
    }
}
