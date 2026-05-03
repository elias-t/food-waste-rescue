using FoodWasteRescue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodWasteRescue.Infrastructure.Persistence.Configurations;

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.Listing)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.ListingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Claimer)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.ClaimerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
