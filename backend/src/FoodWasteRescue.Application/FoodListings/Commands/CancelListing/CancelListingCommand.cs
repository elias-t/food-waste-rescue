using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Commands.CancelListing;

public record CancelListingCommand(Guid ListingId) : IRequest<Result>;
