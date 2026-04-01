using AniRay.Model.Data;
using AniRay.Services.HelperServices.NotificationThing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class BluRayNotificationScheduler : BackgroundService
{
    private readonly ILogger<BluRayNotificationScheduler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BluRayNotificationService _notificationService;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(20);

    public BluRayNotificationScheduler(
        ILogger<BluRayNotificationScheduler> logger,
        IServiceScopeFactory scopeFactory,
        BluRayNotificationService notificationService)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);
        _logger.LogInformation("BluRayNotificationScheduler started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();

                var triggers = await db.BluRayNotificationTriggers
                    .Where(t => t.Trigger)
                    .Select(t => t.BluRayId)
                    .ToListAsync(stoppingToken);

                if (triggers.Count == 0)
                {
                    _logger.LogDebug("No triggers found.");
                }
                else
                {
                    _logger.LogInformation($"Found {triggers.Count} triggered BluRays.");

                    foreach (var bluRayId in triggers)
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _notificationService.RunNotificationJob(bluRayId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error running BluRayNotificationJob for BluRayId {bluRayId}");
                            }
                        }, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BluRayNotificationScheduler.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}