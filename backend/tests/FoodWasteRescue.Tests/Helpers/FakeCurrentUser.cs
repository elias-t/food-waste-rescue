using FoodWasteRescue.Application.Common.Interfaces;

namespace FoodWasteRescue.Tests.Helpers;

public class FakeCurrentUser : ICurrentUserService
{
    public string? UserId { get; set; }
    public bool IsAuthenticated => UserId is not null;
}
