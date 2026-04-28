namespace FoodWasteRescue.Application.Common.Interfaces;

public interface IClaimConfirmationJob
{
    Task SendAsync(Guid claimId);
}
