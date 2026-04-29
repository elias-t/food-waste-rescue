using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Enums;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Commands.CreateFoodListing;

public record CreateFoodListingCommand(
    string Title,
    string? Description,
    FoodCategory Category,
    string QuantityDescription,
    DateTime ExpiresAt,
    double? Latitude,
    double? Longitude,
    string? Address
) : IRequest<Result<Guid>>;
