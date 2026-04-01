using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.HelperServices.OtherHelpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly string _connectionString;

        public BluRayNotificationService(
            ILogger<BluRayNotificationService> logger,
            IServiceScopeFactory scopeFactory,
            IMessageProducer producer,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _producer = producer;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task RunNotificationJob(int bluRayId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                using var lockCommand = connection.CreateCommand();
                lockCommand.Transaction = transaction;

                lockCommand.CommandText = @"
                    DECLARE @res int;
                    EXEC @res = sp_getapplock 
                        @Resource = @ResourceName, 
                        @LockMode = 'Exclusive', 
                        @LockOwner = 'Transaction', 
                        @LockTimeout = 0;
                    SELECT @res;";

                lockCommand.Parameters.AddWithValue("@ResourceName", $"BluRayNotificationJob_{bluRayId}");

                var lockResult = (int)await lockCommand.ExecuteScalarAsync();

                if (lockResult < 0)
                {
                    _logger.LogInformation($"Job already running for BluRay {bluRayId}. Exiting.");
                    await transaction.RollbackAsync();
                    return;
                }

                _logger.LogInformation($"Lock acquired for BluRay {bluRayId}");
                await Task.Delay(500);


                await Task.Delay(500);
                int batchSize = 20;
                int consecutiveFailures = 0;
                int rabbitMqFailures = 0;
                int maxFailures = 5;

                while (true)
                {
                    if (consecutiveFailures >= maxFailures)
                    {
                        _logger.LogInformation("Max consecutive failures reached. Stopping notification job.");
                        _logger.LogInformation("Emails aren't being queued for some reason, look into this!");
                        break;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();

                    var batch = await db.UserBluRayNotifications
                        .Include(n => n.User)
                        .Include(n => n.BluRay)
                        .Where(n => n.BluRayId == bluRayId && !n.EmailQueued && n.FailureCountBeforeQueueing < 6)
                        .OrderBy(n => n.FailureCountBeforeQueueing)
                        .ThenBy(n => n.UserId)
                        .Take(batchSize)
                        .ToListAsync();

                    if (batch.Count == 0)
                    {
                        _logger.LogInformation("No more users to notify. Job finished.");
                        var trigger = await db.BluRayNotificationTriggers.Where(t => t.BluRayId == bluRayId).FirstOrDefaultAsync();
                        trigger.Trigger = false;
                        await db.SaveChangesAsync();
                        break;
                    }

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
                        consecutiveFailures = 0;
                        rabbitMqFailures = 0;
                        await Task.Delay(500);
                    }
                    catch (RabbitMqUnavailableException ex)
                    {
                        rabbitMqFailures++;
                        _logger.LogError(ex, "RabbitMQ unavailable.");
                        await Task.Delay(500);

                        if (rabbitMqFailures > 5)
                        {
                            _logger.LogError(ex, "RabbitMQ unavailable. Stopping job.");
                            break;
                        }
                    }
                    catch (RabbitMqTimeoutException ex)
                    {
                        rabbitMqFailures++;
                        _logger.LogError(ex, "RabbitMQ timeout.");
                        await Task.Delay(500);

                        if (rabbitMqFailures > 5)
                        {
                            _logger.LogError(ex, "RabbitMQ timeout. Stopping job.");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        foreach (var item in batch)
                        {
                            item.FailureCountBeforeQueueing++;
                        }
                        await db.SaveChangesAsync();
                        consecutiveFailures++;
                        _logger.LogError(ex, $"Error sending batch. Failure {consecutiveFailures}/{maxFailures}");
                        await Task.Delay(500);
                    }
                }
                _logger.LogInformation("All batches processed. Job complete.");
                _logger.LogInformation($"Committing transaction for BluRay {bluRayId}");
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in BluRayNotificationJob for BluRay {bluRayId}");
                _logger.LogInformation($"Rolling back transaction for BluRay {bluRayId}");
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                _logger.LogInformation($"Lock released for BluRay {bluRayId}");
                await connection.CloseAsync();
            }
        }
    }
}