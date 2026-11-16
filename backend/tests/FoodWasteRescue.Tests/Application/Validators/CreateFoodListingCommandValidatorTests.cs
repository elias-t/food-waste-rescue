using FluentAssertions;
using FoodWasteRescue.Application.FoodListings.Commands.CreateFoodListing;
using FoodWasteRescue.Domain.Enums;

namespace FoodWasteRescue.Tests.Application.Validators;

public class CreateFoodListingCommandValidatorTests
{
    private readonly CreateFoodListingCommandValidator _sut = new();

    private static CreateFoodListingCommand Valid() => new(
        "Fresh Bread", "Sourdough loaves", FoodCategory.Bakery,
        "5 loaves", DateTime.UtcNow.AddDays(1), null, null, null);

    [Fact]
    public void Valid_command_passes()
    {
        _sut.Validate(Valid()).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_title_fails()
    {
        _sut.Validate(Valid() with { Title = "" }).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Title_over_200_chars_fails()
    {
        _sut.Validate(Valid() with { Title = new string('x', 201) }).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Past_expires_at_fails()
    {
        _sut.Validate(Valid() with { ExpiresAt = DateTime.UtcNow.AddMinutes(-1) }).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-91.0)]
    [InlineData(91.0)]
    public void Out_of_range_latitude_fails(double lat)
    {
        _sut.Validate(Valid() with { Latitude = lat }).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-181.0)]
    [InlineData(181.0)]
    public void Out_of_range_longitude_fails(double lon)
    {
        _sut.Validate(Valid() with { Longitude = lon }).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Empty_description_fails()
    {
        _sut.Validate(Valid() with { Description = "" }).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Null_coordinates_pass()
    {
        _sut.Validate(Valid() with { Latitude = null, Longitude = null }).IsValid.Should().BeTrue();
    }
}
