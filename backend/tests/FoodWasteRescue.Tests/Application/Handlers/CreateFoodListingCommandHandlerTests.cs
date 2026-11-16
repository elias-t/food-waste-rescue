using FluentAssertions;
using FoodWasteRescue.Application.FoodListings.Commands.CreateFoodListing;
using FoodWasteRescue.Domain.Enums;
using FoodWasteRescue.Tests.Helpers;

namespace FoodWasteRescue.Tests.Application.Handlers;

public class CreateFoodListingCommandHandlerTests : IDisposable
{
    private readonly DbContextFactory _factory = new();
    private readonly FakeCurrentUser _currentUser = new();
    private readonly CreateFoodListingCommandHandler _sut;

    public CreateFoodListingCommandHandlerTests()
    {
        _sut = new CreateFoodListingCommandHandler(_factory.Context, _currentUser);
    }

    private static CreateFoodListingCommand ValidCommand() => new(
        "Fresh Bread", "Sourdough loaves", FoodCategory.Bakery,
        "5 loaves", DateTime.UtcNow.AddDays(1), 51.5, -0.1, "London");

    [Fact]
    public async Task Handle_AuthenticatedUser_PersistsListingAndReturnsId()
    {
        var user = await _factory.SeedUserAsync("donor-1");
        _currentUser.UserId = user.Id;

        var result = await _sut.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _factory.Context.FoodListings.Should().ContainSingle(l => l.Id == result.Value);
    }

    [Fact]
    public async Task Handle_UnauthenticatedUser_ReturnsFailure()
    {
        _currentUser.UserId = null;

        var result = await _sut.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    public void Dispose() => _factory.Dispose();
}
