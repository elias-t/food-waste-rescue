using FoodWasteRescue.Domain.Entities;

namespace FoodWasteRescue.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}
