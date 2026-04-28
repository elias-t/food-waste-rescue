using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Commands.ClaimFoodListing;

public record ClaimFoodListingCommand(
    Guid ListingId,
    string? Notes
) : IRequest<Result<Guid>>;
