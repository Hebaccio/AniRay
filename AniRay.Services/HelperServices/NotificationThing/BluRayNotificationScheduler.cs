using AniRay.Model.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public class BluRayNotificationScheduler : BackgroundService
    {
        private readonly ILogger<BluRayNotificationScheduler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly BluRayNotificationService _notificationService;
        private readonly string _connectionString;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(20);

        public BluRayNotificationScheduler(
        ILogger<BluRayNotificationScheduler> logger,
        IServiceScopeFactory scopeFactory,
        BluRayNotificationService notificationService,
        IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _notificationService = notificationService;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
            _logger.LogInformation("BluRayNotificationScheduler started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync(stoppingToken);

                    using var lockCommand = connection.CreateCommand();
                    lockCommand.CommandText = @"
                        DECLARE @res int;
                        EXEC @res = sp_getapplock 
                            @Resource = 'BluRayNotificationJob', 
                            @LockMode = 'Exclusive', 
                            @LockOwner = 'Session', 
                            @LockTimeout = 0;
                        SELECT @res;";

                    var lockResult = (int)await lockCommand.ExecuteScalarAsync(stoppingToken);

                    if (lockResult < 0)
                    {
                        _logger.LogInformation("Another instance is running. Skipping this interval.");
                        await Task.Delay(_interval, stoppingToken);
                        continue;
                    }

                    _logger.LogInformation("Acquired DB lock. Checking triggers...");

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();

                    var triggers = await db.BluRayNotificationTriggers
                        .Where(t => t.Trigger)
                        .ToListAsync(stoppingToken);

                    if (!triggers.Any())
                    {
                        _logger.LogDebug("No triggers found. Releasing lock and waiting.");
                        await Task.Delay(_interval, stoppingToken);
                        continue;
                    }

                    _logger.LogInformation($"Processing {triggers.Count} triggered BluRays.");

                    var tasks = triggers.Select(trigger => Task.Run(async () =>
                    {
                        try
                        {
                            await _notificationService.RunNotificationJob(trigger.BluRayId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error running BluRayNotificationJob for BluRayId {trigger.BluRayId}");
                        }
                    })).ToList();

                    // Wait for all jobs to finish before releasing the DB lock
                    await Task.WhenAll(tasks);

                    // Now you can mark triggers as done if needed
                    foreach (var trigger in triggers)
                    {
                        trigger.Trigger = false;
                    }

                    await db.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Finished processing current batch. Lock will be released automatically.");
                }
                        // Immediately mark trigger false so it won't fire again
                        //trigger.Trigger = false;
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in BluRayNotificationScheduler.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}