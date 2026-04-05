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
    private readonly BluRayNotificationFailedService _failedService;
    private readonly BluRayNotificationDLService _dlService;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public BluRayNotificationScheduler(
        ILogger<BluRayNotificationScheduler> logger,
        IServiceScopeFactory scopeFactory,
        BluRayNotificationService notificationService,
        BluRayNotificationFailedService failedService,
        BluRayNotificationDLService dlService)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _notificationService = notificationService;
        _failedService = failedService;
        _dlService = dlService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000, stoppingToken);
        _logger.LogInformation("BluRayNotificationScheduler started.");

        int _failedWorkerRunning = 0;
        int _dlWorkerRunning = 0;

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
                                await _notificationService.RunNotificationJob(bluRayId, "bluray_notifications_email_queue");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error running BluRayNotificationJob for BluRayId {bluRayId}");
                            }
                        });
                    }
                }

                if (Interlocked.CompareExchange(ref _failedWorkerRunning, 1, 0) == 0)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            _logger.LogInformation("Starting Failed Notification Worker");

                            await _failedService.RunNotificationJob("bluray_notifications_email_queue");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error running Failed Notification Job");
                        }
                        finally
                        {
                            Interlocked.Exchange(ref _failedWorkerRunning, 0);
                            _logger.LogInformation("Failed Notification Worker finished");
                        }
                    });
                }

                if (Interlocked.CompareExchange(ref _dlWorkerRunning, 1, 0) == 0)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            _logger.LogInformation("Starting Dead Letter Notification Worker");

                            await _dlService.RunNotificationJob("bluray_notifications_email_queue");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error running Dead Letter Notification Job");
                        }
                        finally
                        {
                            Interlocked.Exchange(ref _dlWorkerRunning, 0);
                            _logger.LogInformation("Dead Letter Notification Worker finished");
                        }
                    });
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