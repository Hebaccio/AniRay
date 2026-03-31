using AniRay.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public class BluRayNotificationService
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

        public async Task RunNotificationJob(int bluRayId)
        {
            _logger.LogInformation($"+++++++++++++++++++++++++++++++++++++++++++++++++");
            _logger.LogInformation($"Notification job triggered for BluRay {bluRayId}.");
            _logger.LogInformation($"+++++++++++++++++++++++++++++++++++++++++++++++++");

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();

            var usersToNotify = await db.UserBluRayNotifications
                .Include(n => n.User)
                .Include(n => n.BluRay)
                .Where(n => n.BluRayId == bluRayId && !n.EmailSent && !n.EmailQueued)
                .ToListAsync();

            if (usersToNotify.Count == 0)
            {
                _logger.LogInformation("No users to notify. Job finished.");
                return;
            }

            _logger.LogInformation($"Found {usersToNotify.Count} users to notify for BluRay {bluRayId}.");

            int batchSize = 1;
            var batches = usersToNotify
                .Select((user, index) => new { user, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.user).ToList())
                .ToList();

            foreach (var batch in batches)
            {
                try
                {
                    var emailJobs = batch.Select(u => new EmailJobDto
                    {
                        ToEmail = u.User.Email,
                        Subject = $"BluRay Back in Stock: {u.BluRay.Title}",
                        Body = $"Hello {u.User.Name} {u.User.LastName},<br>The BluRay \"{u.BluRay.Title}\" is now available in stock!",
                        UserId = u.UserId,
                        BluRayId = u.BluRayId
                    }).ToList();

                    await _producer.SendMessage("email_queue", emailJobs);
                    _logger.LogInformation($"Batch of {emailJobs.Count} email jobs sent to queue.");

                    foreach (var notif in batch)
                    {
                        notif.EmailQueued = true;
                    }

                    await db.SaveChangesAsync();

                    await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending batch. Continuing with next batch.");
                }
            }

            _logger.LogInformation("All batches processed. Job complete.");
        }
    }
}