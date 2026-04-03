using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.HelperServices.MailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public class EmailConsumerService : BackgroundService
    {
        private readonly ILogger<EmailConsumerService> _logger;
        private readonly RabbitMqDetails _rabbitMqDetails;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly SemaphoreSlim _semaphore = new(5);

        public EmailConsumerService(
            ILogger<EmailConsumerService> logger,
            RabbitMqDetails rabbitMqDetails,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _rabbitMqDetails = rabbitMqDetails;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(2000, cancellationToken);

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqDetails.Host,
                UserName = _rabbitMqDetails.User,
                Password = _rabbitMqDetails.Password,
                VirtualHost = _rabbitMqDetails.VirtualHost,
            };

            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync();

            await SetupQueueAsync(channel);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
                await HandleMessageAsync(channel, eventArgs);

            await channel.BasicConsumeAsync(
                queue: "bluray_notifications_email_queue",
                autoAck: false,
                consumer: consumer);

            while (!cancellationToken.IsCancellationRequested)
                await Task.Delay(1000, cancellationToken);
        }

        private async Task SetupQueueAsync(IChannel channel)
        {
            await channel.QueueDeclareAsync(
                queue: "bluray_notifications_email_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.BasicQosAsync(
                prefetchSize: 0, 
                prefetchCount: 1, 
                global: false);
        }
        private async Task HandleMessageAsync(IChannel channel, BasicDeliverEventArgs eventArgs)
        {
            var emailJobs = DeserializeMessage(eventArgs.Body.ToArray());
            if (emailJobs == null || emailJobs.Count == 0)
            {
                await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();
            var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

            var allNotifications = await FetchNotificationsAsync(db, emailJobs);
            var notificationLookup = allNotifications.ToDictionary(n => (n.UserId, n.BluRayId));

            var failedNotifications = new ConcurrentBag<UserBluRayNotifications>();
            var notificationsToRemove = new ConcurrentBag<UserBluRayNotifications>();

            var tasks = emailJobs.Select(job => 
            ProcessEmailJobAsync(job, mailService, notificationLookup, failedNotifications, notificationsToRemove));
            await Task.WhenAll(tasks);

            await SaveResultsAsync(db, failedNotifications, notificationsToRemove);

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        }
        private static List<EmailJobDto>? DeserializeMessage(byte[] body)
        {
            var messageString = Encoding.UTF8.GetString(body);
            return JsonSerializer.Deserialize<List<EmailJobDto>>(messageString);
        }
        private static async Task<List<UserBluRayNotifications>> FetchNotificationsAsync(
            AniRayDbContext db, List<EmailJobDto> emailJobs)
        {
            var userIds = emailJobs.Select(j => j.UserId).Distinct().ToList();
            var bluRayIds = emailJobs.Select(j => j.BluRayId).Distinct().ToList();

            return await db.UserBluRayNotifications
                .Where(n => userIds.Contains(n.UserId) && bluRayIds.Contains(n.BluRayId))
                .ToListAsync();
        }
        private async Task ProcessEmailJobAsync(
            EmailJobDto job,
            IMailService mailService,
            Dictionary<(int, int), UserBluRayNotifications> notificationLookup,
            ConcurrentBag<UserBluRayNotifications> failedNotifications,
            ConcurrentBag<UserBluRayNotifications> notificationsToRemove)
        {
            await _semaphore.WaitAsync();
            try
            {
                await mailService.SendEmailAsync(job.ToEmail, job.Subject, job.Body);
                _logger.LogInformation($"Email sent to: {job.ToEmail}");

                if (notificationLookup.TryGetValue((job.UserId, job.BluRayId), out var notification))
                    notificationsToRemove.Add(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {job.ToEmail}");
                if (notificationLookup.TryGetValue((job.UserId, job.BluRayId), out var notification))
                    failedNotifications.Add(notification);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private static async Task SaveResultsAsync(
            AniRayDbContext db,
            ConcurrentBag<UserBluRayNotifications> failedNotifications,
            ConcurrentBag<UserBluRayNotifications> notificationsToRemove)
        {
            if (notificationsToRemove.Count > 0)
                db.RemoveRange(notificationsToRemove);

            foreach (var fail in failedNotifications)
                fail.FailureCountBeforeSending++;

            if (notificationsToRemove.Count > 0 || failedNotifications.Count > 0)
                await db.SaveChangesAsync();
        }
    }
}