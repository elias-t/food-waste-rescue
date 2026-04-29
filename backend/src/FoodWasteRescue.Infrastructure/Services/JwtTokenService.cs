using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SecurityClaim = System.Security.Claims.Claim;
using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoodWasteRescue.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(ApplicationUser user)
    {
        var secret = _configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JwtSettings:Secret is not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new SecurityClaim(JwtRegisteredClaimNames.Sub, user.Id),
            new SecurityClaim(JwtRegisteredClaimNames.Email, user.Email!),
            new SecurityClaim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new SecurityClaim(ClaimTypes.NameIdentifier, user.Id),
            new SecurityClaim(ClaimTypes.Role, user.Role.ToString()),
            new SecurityClaim("displayName", user.DisplayName ?? string.Empty)
        };

        var expiryDays = int.TryParse(_configuration["JwtSettings:ExpiryDays"], out var days) ? days : 7;

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expiryDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
