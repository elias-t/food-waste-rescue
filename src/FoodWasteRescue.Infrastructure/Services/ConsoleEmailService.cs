using FoodWasteRescue.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoodWasteRescue.Infrastructure.Services;

public class ConsoleEmailService(ILogger<ConsoleEmailService> logger) : IEmailService
{
    public Task SendAsync(string to, string subject, string body,
        CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL] To: {To} | Subject: {Subject} | Body: {Body}",
            to, subject, body);
        return Task.CompletedTask;
    }
}
