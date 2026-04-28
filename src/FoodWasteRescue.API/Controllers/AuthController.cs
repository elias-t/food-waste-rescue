using FoodWasteRescue.Application.Auth.Commands.LoginUser;
using FoodWasteRescue.Application.Auth.Commands.RegisterUser;
using FoodWasteRescue.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodWasteRescue.API.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new RegisterUserCommand(request.Email, request.Password, request.DisplayName, request.OrganisationName, request.Role),
            ct);

        return result.IsSuccess ? Ok(new { token = result.Value }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new LoginUserCommand(request.Email, request.Password), ct);

        return result.IsSuccess ? Ok(new { token = result.Value }) : BadRequest(new { error = result.Error });
    }
}

public record RegisterRequest(string Email, string Password, string DisplayName, string? OrganisationName, UserRole Role);
public record LoginRequest(string Email, string Password);
