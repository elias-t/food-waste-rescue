using FluentAssertions;
using FoodWasteRescue.Application.Auth.Commands.RegisterUser;
using FoodWasteRescue.Domain.Enums;

namespace FoodWasteRescue.Tests.Application.Validators;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _sut = new();

    private static RegisterUserCommand Valid() =>
        new("test@example.com", "password123", "Test User", null, UserRole.Donor);

    [Fact]
    public void Valid_command_passes()
    {
        _sut.Validate(Valid()).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Invalid_email_fails(string email)
    {
        _sut.Validate(Valid() with { Email = email }).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    public void Invalid_password_fails(string password)
    {
        _sut.Validate(Valid() with { Password = password }).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_display_name_fails()
    {
        _sut.Validate(Valid() with { DisplayName = "" }).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Display_name_over_100_chars_fails()
    {
        _sut.Validate(Valid() with { DisplayName = new string('a', 101) }).IsValid.Should().BeFalse();
    }
}
