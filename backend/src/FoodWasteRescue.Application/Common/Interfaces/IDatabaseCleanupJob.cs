namespace FoodWasteRescue.Application.Common.Interfaces;

public interface IDatabaseCleanupJob
{
    Task ExecuteAsync();
}
