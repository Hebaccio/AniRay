using AniRay.Model.Data;
using AniRay.Services.HelperServices.MailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public class FailedEmailRetryService : BackgroundService
    {
        private readonly ILogger<FailedEmailRetryService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly int _batchSize = 20;
        private readonly int _maxConcurrentEmails = 5;
        private readonly TimeSpan _delayBetweenBatches = TimeSpan.FromMinutes(1);

        public FailedEmailRetryService(
            ILogger<FailedEmailRetryService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FailedEmailRetryService started.");
            var semaphore = new SemaphoreSlim(_maxConcurrentEmails);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();
                    var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

                    var failedBatch = await db.UserBluRayNotifications
                        .Include(n => n.User)
                        .Include(n => n.BluRay)
                        .Where(n => n.FailureCountBeforeSending >= 1)
                        .OrderBy(n => n.FailureCountBeforeSending)
                        .ThenBy(n => n.UserId)
                        .Take(_batchSize)
                        .ToListAsync(stoppingToken);

                    if (failedBatch.Count == 0)
                    {
                        _logger.LogDebug("No failed notifications to retry. Waiting...");
                        await Task.Delay(_delayBetweenBatches, stoppingToken);
                        continue;
                    }

                    _logger.LogInformation($"Retrying batch of {failedBatch.Count} failed notifications.");

                    var tasks = new List<Task>();

                    foreach (var notification in failedBatch)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            await semaphore.WaitAsync(stoppingToken);
                            try
                            {
                                var toEmail = notification.User.Email;
                                var subject = $"BluRay Back in Stock: {notification.BluRay.Title}";
                                var body = $"Hello {notification.User.Name} {notification.User.LastName},<br>The BluRay \"{notification.BluRay.Title}\" is now available in stock!";

                                try
                                {
                                    await mailService.SendEmailAsync(toEmail, subject, body);
                                    _logger.LogInformation($"Successfully resent email to {toEmail}.");

                                    db.UserBluRayNotifications.Remove(notification);
                                }
                                catch (Exception ex)
                                {
                                    notification.FailureCountBeforeSending++;
                                    _logger.LogError(ex, $"Failed to resend email to {toEmail}. Failure count is now {notification.FailureCountBeforeSending}");
                                }
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }, stoppingToken));
                    }

                    await Task.WhenAll(tasks);
                    await db.SaveChangesAsync(stoppingToken);
                    await Task.Delay(_delayBetweenBatches, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in FailedEmailRetryService.");
                    await Task.Delay(_delayBetweenBatches, stoppingToken);
                }
            }
        }
    }
}