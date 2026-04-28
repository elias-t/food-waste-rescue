using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetMyListings;

public record GetMyListingsQuery : IRequest<Result<List<MyListingDto>>>;
