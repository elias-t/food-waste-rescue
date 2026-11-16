using FluentAssertions;
using FoodWasteRescue.Domain.Entities;
using FoodWasteRescue.Domain.Enums;

namespace FoodWasteRescue.Tests.Domain;

public class BaseEntityTests
{
    private static FoodListing Make() => new(
        "Title", null, FoodCategory.Other, "1 bag", DateTime.UtcNow.AddDays(1), "donor");

    [Fact]
    public void NewEntity_HasNonEmptyId()
    {
        Make().Id.Should().NotBeEmpty();
    }

    [Fact]
    public void NewEntity_HasCreatedAtSet()
    {
        var before = DateTime.UtcNow;
        Make().CreatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void TwoEntities_HaveDifferentIds()
    {
        Make().Id.Should().NotBe(Make().Id);
    }
}
