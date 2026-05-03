namespace FoodWasteRescue.Application.Common.Interfaces;

public interface IClaimReminderJob
{
    Task SendReminderAsync(Guid claimId);
}
