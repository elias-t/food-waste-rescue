using FluentAssertions;
using FoodWasteRescue.Application.Auth.Commands.LoginUser;

namespace FoodWasteRescue.Tests.Application.Validators;

public class LoginUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _sut = new();

    [Fact]
    public void Valid_command_passes()
    {
        _sut.Validate(new LoginUserCommand("test@example.com", "password123")).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_email_fails()
    {
        _sut.Validate(new LoginUserCommand("", "password123")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_password_fails()
    {
        _sut.Validate(new LoginUserCommand("test@example.com", "")).IsValid.Should().BeFalse();
    }
}
