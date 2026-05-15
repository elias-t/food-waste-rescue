using FoodWasteRescue.Application;
using FoodWasteRescue.Application.Common.Interfaces;
using FoodWasteRescue.Infrastructure;
using FoodWasteRescue.Worker.Jobs;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

// Register job classes
builder.Services.AddScoped<IExpireListingsJob, ExpireListingsJob>();
builder.Services.AddScoped<IDailyImpactDigestJob, DailyImpactDigestJob>();
builder.Services.AddScoped<IUnclaimedAlertJob, UnclaimedAlertJob>();
builder.Services.AddScoped<IDatabaseCleanupJob, DatabaseCleanupJob>();
builder.Services.AddScoped<IClaimConfirmationJob, ClaimConfirmationJob>();
builder.Services.AddScoped<IClaimReminderJob, ClaimReminderJob>();

var host = builder.Build();

// Register recurring jobs
using (var scope = host.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<IExpireListingsJob>(
        "expire-listings", j => j.ExecuteAsync(), Cron.Hourly);

    recurringJobManager.AddOrUpdate<IDailyImpactDigestJob>(
        "daily-digest", j => j.ExecuteAsync(), "0 8 * * *");

    recurringJobManager.AddOrUpdate<IUnclaimedAlertJob>(
        "unclaimed-alert", j => j.ExecuteAsync(), "0 7 * * *");

    recurringJobManager.AddOrUpdate<IDatabaseCleanupJob>(
        "db-cleanup", j => j.ExecuteAsync(), "0 2 * * 0");
}

await host.RunAsync();
