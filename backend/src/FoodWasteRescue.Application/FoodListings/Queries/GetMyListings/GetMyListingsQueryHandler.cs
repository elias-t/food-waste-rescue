using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.FoodListings.Queries.GetMyListings;

public class GetMyListingsQueryHandler : IRequestHandler<GetMyListingsQuery, Result<List<MyListingDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyListingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<MyListingDto>>> Handle(GetMyListingsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<List<MyListingDto>>.Failure("User not authenticated");

        var listings = await _context.FoodListings
            .AsNoTracking()
            .Where(l => l.DonorId == userId)
            .Select(l => new MyListingDto
            {
                Id = l.Id,
                Title = l.Title,
                Category = l.Category,
                Status = l.Status,
                ExpiresAt = l.ExpiresAt,
                ClaimCount = l.Claims.Count,
                CreatedAt = l.CreatedAt
            })
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<List<MyListingDto>>.Success(listings);
    }
}
