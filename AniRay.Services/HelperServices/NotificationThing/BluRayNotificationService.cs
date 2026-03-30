using AniRay.Model.Data;
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
    public class BluRayNotificationService : BackgroundService
    {
        private readonly ILogger<BluRayNotificationService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessageProducer _producer;

        public BluRayNotificationService(
            ILogger<BluRayNotificationService> logger,
            IServiceScopeFactory scopeFactory,
            IMessageProducer producer)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _producer = producer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This is just a loop waiting for "triggered jobs"
            // We'll trigger it by calling AddNotificationJob(...) from your BluRay update
            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait briefly to reduce CPU usage
                await Task.Delay(500, stoppingToken);
            }
        }

        // This method can be called by your BluRay update
        public async Task AddNotificationJob(int bluRayId)
        {
            await Task.Delay(5000);
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();

            var usersToNotify = await db.UserBluRayNotifications
                .Include(n => n.User).Include(n=> n.BluRay)
                .Where(n => n.BluRayId == bluRayId && !n.EmailSent)
                .ToListAsync();

            // For now, just log to verify it works
            _logger.LogInformation($"Trigger received for BluRay {bluRayId}, {usersToNotify.Count} users to notify.");

            // Later: create EmailJob batches & push to RabbitMQ here
            int batchSize = 100; // adjust as needed
            var batches = usersToNotify
                .Select((user, index) => new { user, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.user).ToList())
                .ToList();

            foreach (var batch in batches)
            {
                var emailJobs = batch.Select(u => new EmailJobDto
                {
                    ToEmail = u.User.Email,
                    Subject = $"BluRay Back in Stock: {u.BluRay.Title}",
                    Body = $"Hello {u.User.Name},<br>The BluRay \"{u.BluRay.Title}\" is now available in stock!"
                }).ToList();

                // Send this batch to RabbitMQ
                await _producer.SendMessage("email_queue", emailJobs);
                _logger.LogInformation($"Batch of {emailJobs.Count} email jobs sent to queue.");
            }
        }
    }
}
