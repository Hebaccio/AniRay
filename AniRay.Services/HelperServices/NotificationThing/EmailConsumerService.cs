using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.HelperServices.MailService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public class EmailConsumerService : BackgroundService
    {
        private readonly ILogger<EmailConsumerService> _logger;
        private readonly RabbitMqDetails _rabbitMqDetails;
        private readonly IServiceScopeFactory _scopeFactory;

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
            await Task.Delay(2000);

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqDetails.Host,
                UserName = _rabbitMqDetails.User,
                VirtualHost = _rabbitMqDetails.VirtualHost,
                Password = _rabbitMqDetails.Password,
            };

            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "email_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            var semaphore = new SemaphoreSlim(5);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var messageString = Encoding.UTF8.GetString(body);

                var emailJobs = JsonSerializer.Deserialize<List<EmailJobDto>>(messageString);
                if (emailJobs != null && emailJobs.Count > 0)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();
                    var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

                    var failedNotifications = new List<UserBluRayNotifications>();
                    var notificationsToRemove = new List<UserBluRayNotifications>();

                    var tasks = emailJobs.Select(async job =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            await mailService.SendEmailAsync(job.ToEmail, job.Subject, job.Body);
                            _logger.LogInformation($"Email sent to: {job.ToEmail}");

                            var notification = await db.UserBluRayNotifications
                                .FirstOrDefaultAsync(n => n.UserId == job.UserId && n.BluRayId == job.BluRayId);

                            if (notification != null)
                                notificationsToRemove.Add(notification);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to send email to {job.ToEmail}");

                            var notification = await db.UserBluRayNotifications
                                .FirstOrDefaultAsync(n => n.UserId == job.UserId && n.BluRayId == job.BluRayId);

                            if (notification != null)
                                failedNotifications.Add(notification);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    await Task.WhenAll(tasks);

                    //if (notificationsToRemove.Count > 0)
                    //    db.RemoveRange(notificationsToRemove);

                    foreach (var fail in notificationsToRemove)
                        fail.EmailFailed = false;

                    foreach (var fail in failedNotifications)
                        fail.EmailFailed = true;

                    if (notificationsToRemove.Count > 0 || failedNotifications.Count > 0)
                        await db.SaveChangesAsync();
                }

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(
                queue: "email_queue",
                autoAck: false,
                consumer: consumer);

            while (!cancellationToken.IsCancellationRequested)
                await Task.Delay(1000, cancellationToken);
        }
    }
}