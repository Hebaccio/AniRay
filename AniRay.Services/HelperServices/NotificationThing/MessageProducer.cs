using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public class MessageProducer : IMessageProducer
    {
        private readonly ILogger<MessageProducer> _logger;
        private readonly RabbitMqDetails _rabbitMqDetails;

        public MessageProducer(ILogger<MessageProducer> logger, RabbitMqDetails rabbitMqDetails)
        {
            _logger = logger;
            _rabbitMqDetails = rabbitMqDetails;
        }

        public async Task SendMessage<T>(string queue, T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqDetails.Host,
                UserName = _rabbitMqDetails.User,
                VirtualHost = _rabbitMqDetails.VirtualHost,
                Password = _rabbitMqDetails.Password,
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue,
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);

            _logger.LogInformation($"+++++++++++++++++++++++++++++++++++++++++++++++++");
            _logger.LogDebug($"Sent message on queue {queue} : {jsonString}");
            _logger.LogInformation($"+++++++++++++++++++++++++++++++++++++++++++++++++");
        }
    }
}
