using FluentAssertions;
using FoodWasteRescue.Application.Auth.Commands.RegisterUser;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Tests.Helpers;

namespace FoodWasteRescue.Tests.Application.Handlers;

public class RegisterUserCommandHandlerTests : IDisposable
{
    private readonly IdentityTestSetup _setup = new();
    private readonly RegisterUserCommandHandler _sut;

    public RegisterUserCommandHandlerTests()
    {
        _sut = new RegisterUserCommandHandler(_setup.IdentityService);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsToken()
    {
        var result = await _sut.Handle(
            new RegisterUserCommand("new@example.com", "testpass1", "New User", null, UserRole.Donor),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ReturnsFailure()
    {
        var cmd = new RegisterUserCommand("dup@example.com", "testpass1", "User", null, UserRole.Donor);
        await _sut.Handle(cmd, CancellationToken.None);

        var result = await _sut.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    public void Dispose() => _setup.Dispose();
}
