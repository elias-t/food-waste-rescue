using FoodWasteRescue.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodWasteRescue.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.OrganisationName)
            .HasMaxLength(200);

        builder.Property(x => x.Bio)
            .HasMaxLength(500);

        builder.Property(x => x.Role)
            .HasConversion<string>();
    }
}
