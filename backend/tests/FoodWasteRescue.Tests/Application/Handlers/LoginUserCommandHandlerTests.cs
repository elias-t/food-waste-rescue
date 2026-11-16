using FluentAssertions;
using FoodWasteRescue.Application.Auth.Commands.LoginUser;
using FoodWasteRescue.Application.Auth.Commands.RegisterUser;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Tests.Helpers;

namespace FoodWasteRescue.Tests.Application.Handlers;

public class LoginUserCommandHandlerTests : IDisposable
{
    private const string TestEmail = "login@example.com";
    private const string TestPassword = "testpass1";

    private readonly IdentityTestSetup _setup = new();
    private readonly LoginUserCommandHandler _sut;

    public LoginUserCommandHandlerTests()
    {
        _sut = new LoginUserCommandHandler(_setup.IdentityService);
    }

    private async Task SeedUserAsync()
    {
        var register = new RegisterUserCommandHandler(_setup.IdentityService);
        await register.Handle(
            new RegisterUserCommand(TestEmail, TestPassword, "Login User", null, UserRole.Donor),
            CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        await SeedUserAsync();

        var result = await _sut.Handle(new LoginUserCommand(TestEmail, TestPassword), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        await SeedUserAsync();

        var result = await _sut.Handle(new LoginUserCommand(TestEmail, "wrongpassword"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_UnknownEmail_ReturnsFailure()
    {
        var result = await _sut.Handle(new LoginUserCommand("nobody@example.com", TestPassword), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    public void Dispose() => _setup.Dispose();
}
