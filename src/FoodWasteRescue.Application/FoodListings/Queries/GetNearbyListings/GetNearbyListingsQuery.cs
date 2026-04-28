using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetNearbyListings;

public record GetNearbyListingsQuery(
    double Latitude,
    double Longitude,
    double RadiusKm = 10
) : IRequest<Result<List<NearbyListingDto>>>;
