using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Entities;
using MediatR;

namespace FoodWasteRescue.Application.FoodListings.Commands.CreateFoodListing;

public class CreateFoodListingCommandHandler : IRequestHandler<CreateFoodListingCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateFoodListingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreateFoodListingCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Result<Guid>.Failure("User not authenticated");

        var listing = new FoodListing(
            request.Title,
            request.Description,
            request.Category,
            request.QuantityDescription,
            request.ExpiresAt,
            _currentUserService.UserId,
            request.Latitude,
            request.Longitude,
            request.Address);

        _context.FoodListings.Add(listing);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(listing.Id);
    }
}
