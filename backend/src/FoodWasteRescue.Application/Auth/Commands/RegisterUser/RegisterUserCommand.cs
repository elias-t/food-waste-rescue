using FoodWasteRescue.Application.Common.Models;
using FoodWasteRescue.Domain.Enums;
using MediatR;

namespace FoodWasteRescue.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password,
    string DisplayName,
    string? OrganisationName,
    UserRole Role
) : IRequest<Result<string>>;
