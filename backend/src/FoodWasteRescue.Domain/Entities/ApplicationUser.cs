using FoodWasteRescue.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace FoodWasteRescue.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? OrganisationName { get; set; }
    public string? Bio { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Address { get; set; }
    public UserRole Role { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<FoodListing> FoodListings { get; set; } = [];
    public ICollection<Claim> Claims { get; set; } = [];
}
