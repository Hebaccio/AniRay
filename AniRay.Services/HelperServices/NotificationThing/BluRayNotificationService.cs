using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.HelperServices.OtherHelpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        public async Task RunNotificationJob(int bluRayId, string queueName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                if (!await AcquireLock(connection, transaction, bluRayId))
                {
                    _logger.LogInformation($"Job already running for BluRay {bluRayId}. Exiting.");
                    await transaction.RollbackAsync();
                    return;
                }

                _logger.LogInformation($"Lock acquired for BluRay {bluRayId}");

                await ProcessBatches(bluRayId, queueName);

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

        private async Task<bool> AcquireLock(SqlConnection connection, SqlTransaction transaction, int bluRayId)
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
            return lockResult >= 0;
        }
        private async Task ProcessBatches(int bluRayId, string queueName)
        {
            int batchSize = 15;
            int consecutiveRabbitMqFailures = 0;
            int consecutiveNormalFailures = 0;
            int maxNormalFailures = 10;
            int maxRabbitMqFailures = 5;

            while (true)
            {
                if (FailuresCheck(consecutiveNormalFailures, maxNormalFailures, consecutiveRabbitMqFailures, maxRabbitMqFailures))
                    break;

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AniRayDbContext>();

                var batch = await GetNextBatch(db, bluRayId, batchSize);

                if (batch.Count == 0)
                {
                    await FinishJob(db, bluRayId);
                    break;
                }

                try
                {
                    await SendEmails(batch, queueName);
                    await MarkBatchAsQueued(db, batch);
                    consecutiveRabbitMqFailures = 0;
                    consecutiveNormalFailures = 0;
                }
                catch (Exception ex) when (ex is RabbitMqUnavailableException or RabbitMqTimeoutException)
                {
                    consecutiveRabbitMqFailures++;
                    _logger.LogError(ex, "RabbitMQ unavailable or timed out.");
                }
                catch (Exception ex)
                {
                    consecutiveNormalFailures++;
                    await HandleGeneralFailure(db, batch, consecutiveNormalFailures, ex);
                }

                await Task.Delay(500);
            }
            if(FailuresCheck(consecutiveNormalFailures, maxNormalFailures, consecutiveRabbitMqFailures, maxRabbitMqFailures))
            {
                _logger.LogInformation("Max consecutive failures reached. Stopping notification job.");
                _logger.LogInformation("Emails aren't being queued for some reason, look into this!");
            }
            else
                _logger.LogInformation("All batches processed. Job complete.");
        }
        private async Task<List<UserBluRayNotifications>> GetNextBatch(AniRayDbContext db, int bluRayId, int batchSize)
        {
            return await db.UserBluRayNotifications
                .Include(n => n.User)
                .Include(n => n.BluRay)
                .Where(n => n.BluRayId == bluRayId && !n.EmailQueued && n.FailureCountBeforeQueueing < 3)
                .OrderBy(n => n.FailureCountBeforeQueueing)
                .ThenBy(n => n.UserId)
                .Take(batchSize)
                .ToListAsync();
        }
        private async Task SendEmails(List<UserBluRayNotifications> batch, string queueName)
        {
            var emailJobs = batch.Select(u => new EmailJobDto
            {
                ToEmail = u.User.Email,
                Subject = $"BluRay Back in Stock: {u.BluRay.Title}",
                Body = $"Hello {u.User.Name} {u.User.LastName},<br>The BluRay \"{u.BluRay.Title}\" is now available in stock!",
                UserId = u.UserId,
                BluRayId = u.BluRayId
            }).ToList();

            await _producer.SendMessage(queueName, emailJobs);
        }
        private async Task MarkBatchAsQueued(AniRayDbContext db, List<UserBluRayNotifications> batch)
        {
            foreach (var notif in batch)
                notif.EmailQueued = true;

            await db.SaveChangesAsync();
        }
        private async Task HandleGeneralFailure(AniRayDbContext db, List<UserBluRayNotifications> batch, int failureCount, Exception ex)
        {
            foreach (var item in batch)
                item.FailureCountBeforeQueueing++;

            await db.SaveChangesAsync();
            _logger.LogError(ex, $"Error sending batch. Failure {failureCount}/5");
        }
        private async Task FinishJob(AniRayDbContext db, int bluRayId)
        {
            _logger.LogInformation("No more users to notify. Job finished.");

            var trigger = await db.BluRayNotificationTriggers
                .Where(t => t.BluRayId == bluRayId)
                .FirstOrDefaultAsync();

            if (trigger != null)
            {
                trigger.Trigger = false;
                await db.SaveChangesAsync();
            }
        }
        private bool FailuresCheck(int consecutiveNormalFailures, int maxNormalFailures, int consecutiveRabbitMqFailures, int maxRabbitMqFailures)
        {
            return consecutiveNormalFailures >= maxNormalFailures || consecutiveRabbitMqFailures >= maxRabbitMqFailures;
        }
    }
}