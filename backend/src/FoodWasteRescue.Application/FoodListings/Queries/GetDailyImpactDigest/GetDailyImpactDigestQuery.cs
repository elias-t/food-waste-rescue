using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetDailyImpactDigest;

public record GetDailyImpactDigestQuery : IRequest<Result<DailyDigestDto>>;
