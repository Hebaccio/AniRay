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
            _logger.LogInformation("+++++++++++++++++++++++++++++++++++++++++++++++++");
            _logger.LogInformation($"Notification job triggered for BluRay {bluRayId}.");
            _logger.LogInformation("+++++++++++++++++++++++++++++++++++++++++++++++++");
            await Task.Delay(100000);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();

            int batchSize = 20;

            while (true)
            {
                var batch = await db.UserBluRayNotifications
                    .Include(n => n.User)
                    .Include(n => n.BluRay)
                    .Where(n => n.BluRayId == bluRayId && (!n.EmailQueued || (n.EmailQueued && n.EmailFailed)))
                    .OrderBy(n => n.UserId)
                    .Take(batchSize)
                    .ToListAsync();

                if (batch.Count == 0)
                {
                    _logger.LogInformation("No more users to notify. Job finished.");
                    break;
                }

                _logger.LogInformation($"Processing batch of {batch.Count} users for BluRay {bluRayId}.");

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

                    foreach (var notif in batch)
                        notif.EmailQueued = true;
                    
                    await db.SaveChangesAsync();

                    _logger.LogInformation("Batch processed successfully.");

                    await Task.Delay(1500);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending batch. Retrying next batch...");
                }
            }

            _logger.LogInformation("All batches processed. Job complete.");
        }
    }
}