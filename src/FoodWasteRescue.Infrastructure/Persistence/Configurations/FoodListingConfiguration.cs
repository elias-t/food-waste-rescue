using FoodWasteRescue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodWasteRescue.Infrastructure.Persistence.Configurations;

public class FoodListingConfiguration : IEntityTypeConfiguration<FoodListing>
{
    public void Configure(EntityTypeBuilder<FoodListing> builder)
    {
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.QuantityDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.Category)
            .HasConversion<string>();

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6);

        builder.HasOne(x => x.Donor)
            .WithMany(x => x.FoodListings)
            .HasForeignKey(x => x.DonorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
