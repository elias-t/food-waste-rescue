using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Application.FoodListings.Commands.CleanupOldListings;
using MediatR;

namespace FoodWasteRescue.Worker.Jobs;

public class DatabaseCleanupJob(IMediator mediator) : IDatabaseCleanupJob
{
    public async Task ExecuteAsync()
    {
        await mediator.Send(new CleanupOldListingsCommand());
    }
}
