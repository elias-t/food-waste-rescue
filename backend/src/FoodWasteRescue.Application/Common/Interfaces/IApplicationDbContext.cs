using FoodWasteRescue.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodWasteRescue.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<FoodListing> FoodListings { get; }
    DbSet<Claim> Claims { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
