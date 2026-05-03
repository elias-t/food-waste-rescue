using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Enums;

namespace FoodWasteRescue.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<string>> RegisterAsync(string email, string password,
        string displayName, string? organisationName, UserRole role);

    Task<Result<string>> LoginAsync(string email, string password);
}
