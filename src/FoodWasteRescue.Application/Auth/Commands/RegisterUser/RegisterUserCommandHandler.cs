using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.Common.Models;
using MediatR;

namespace FoodWasteRescue.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken) =>
        _identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.DisplayName,
            request.OrganisationName,
            request.Role);
}
