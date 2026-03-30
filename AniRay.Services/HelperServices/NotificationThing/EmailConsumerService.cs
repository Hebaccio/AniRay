using AniRay.Model.Data;
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

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var messageString = Encoding.UTF8.GetString(body);

                try
                {
                    var emailJobs = JsonSerializer.Deserialize<List<EmailJobDto>>(messageString);
                    if (emailJobs != null && emailJobs.Count > 0)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();
                        var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

                        foreach (var job in emailJobs)
                        {
                            try
                            {
                                await mailService.SendEmailAsync(job.ToEmail, job.Subject, job.Body);

                                var notification = await db.UserBluRayNotifications
                                    .Include(n => n.User)
                                    .Include(n => n.BluRay)
                                    .FirstOrDefaultAsync(n =>
                                        n.User.Email == job.ToEmail &&
                                        n.BluRay.Title == job.Subject);

                                if (notification != null)
                                    notification.EmailSent = true;

                                await db.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Failed to send email to {job.ToEmail}");
                            }
                        }
                    }

                    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process email batch");
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
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