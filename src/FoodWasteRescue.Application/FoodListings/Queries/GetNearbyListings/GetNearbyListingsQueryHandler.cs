using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetNearbyListings;

public class GetNearbyListingsQueryHandler : IRequestHandler<GetNearbyListingsQuery, Result<List<NearbyListingDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetNearbyListingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<NearbyListingDto>>> Handle(GetNearbyListingsQuery request, CancellationToken cancellationToken)
    {
        var radiusKm = Math.Min(request.RadiusKm, 50);

        var listings = await _context.FoodListings
            .AsNoTracking()
            .Where(l => l.Status == ListingStatus.Active && l.Latitude != null && l.Longitude != null)
            .Include(l => l.Donor)
            .ToListAsync(cancellationToken);

        var result = listings
            .Select(l => new NearbyListingDto
            {
                Id = l.Id,
                Title = l.Title,
                Category = l.Category,
                QuantityDescription = l.QuantityDescription,
                ExpiresAt = l.ExpiresAt,
                Address = l.Address,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                DonorName = l.Donor?.DisplayName,
                DistanceKm = Haversine(request.Latitude, request.Longitude, l.Latitude!.Value, l.Longitude!.Value)
            })
            .Where(dto => dto.DistanceKm <= radiusKm)
            .OrderBy(dto => dto.DistanceKm)
            .ToList();

        return Result<List<NearbyListingDto>>.Success(result);
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
