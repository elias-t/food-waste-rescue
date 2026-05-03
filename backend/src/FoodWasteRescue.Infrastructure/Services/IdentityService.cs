using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace FoodWasteRescue.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public IdentityService(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<string>> RegisterAsync(string email, string password,
        string displayName, string? organisationName, UserRole role)
    {
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            DisplayName = displayName,
            OrganisationName = organisationName,
            Role = role
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure(errors);
        }

        return Result<string>.Success(_jwtTokenService.GenerateToken(user));
    }

    public async Task<Result<string>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<string>.Failure("Invalid credentials");

        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            return Result<string>.Failure("Invalid credentials");

        return Result<string>.Success(_jwtTokenService.GenerateToken(user));
    }
}
