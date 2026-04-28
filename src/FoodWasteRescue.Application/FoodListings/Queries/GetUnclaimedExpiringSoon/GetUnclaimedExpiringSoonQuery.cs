using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetUnclaimedExpiringSoon;

public record GetUnclaimedExpiringSoonQuery : IRequest<Result<List<UnclaimedListingDto>>>;
