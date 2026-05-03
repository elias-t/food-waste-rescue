using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetListingDetail;

public record GetListingDetailQuery(Guid ListingId) : IRequest<Result<ListingDetailDto>>;
