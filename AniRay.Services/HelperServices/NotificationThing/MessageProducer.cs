using AniRay.Services.HelperServices.OtherHelpers;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
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
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _rabbitMqDetails.Host,
                    UserName = _rabbitMqDetails.User,
                    VirtualHost = _rabbitMqDetails.VirtualHost,
                    Password = _rabbitMqDetails.Password,
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(10)
                };

                await using var connection = await factory.CreateConnectionAsync();

                if (!connection.IsOpen)
                    throw new RabbitMqUnavailableException("RabbitMQ connection could not be opened.");

                await using var channel = await connection.CreateChannelAsync();

                if (!channel.IsOpen)
                    throw new RabbitMqUnavailableException("RabbitMQ channel could not be opened.");

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

                _logger.LogInformation($"Sent message on queue {queue}");
            }
            catch (BrokerUnreachableException ex)
            {
                throw new RabbitMqUnavailableException("RabbitMQ unreachable.");
            }
            catch (ConnectFailureException ex)
            {
                throw new RabbitMqUnavailableException("RabbitMQ connection failed.");
            }
            catch (TimeoutException ex)
            {
                throw new RabbitMqTimeoutException("RabbitMQ timeout.");
            }
            catch (OperationInterruptedException ex)
            {
                throw new RabbitMqOtherException("RabbitMQ operation interrupted.");
            }
        }
    }
}
