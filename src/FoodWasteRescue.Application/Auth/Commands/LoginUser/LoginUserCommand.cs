using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.Auth.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : IRequest<Result<string>>;
