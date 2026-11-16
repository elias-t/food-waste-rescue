using FluentAssertions;
using FoodWasteRescue.Application.FoodListings.Commands.ClaimFoodListing;

namespace FoodWasteRescue.Tests.Application.Validators;

public class ClaimFoodListingCommandValidatorTests
{
    private readonly ClaimFoodListingCommandValidator _sut = new();

    [Fact]
    public void Valid_command_passes()
    {
        _sut.Validate(new ClaimFoodListingCommand(Guid.NewGuid(), null)).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_listing_id_fails()
    {
        _sut.Validate(new ClaimFoodListingCommand(Guid.Empty, null)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Notes_over_500_chars_fails()
    {
        _sut.Validate(new ClaimFoodListingCommand(Guid.NewGuid(), new string('n', 501))).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Notes_exactly_500_chars_passes()
    {
        _sut.Validate(new ClaimFoodListingCommand(Guid.NewGuid(), new string('n', 500))).IsValid.Should().BeTrue();
    }
}
