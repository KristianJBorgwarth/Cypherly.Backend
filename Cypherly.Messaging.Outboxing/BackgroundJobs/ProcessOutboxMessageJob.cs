using Cypherly.Application.Contracts.Repository;
using Cypherly.Persistence.Repositories;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Cypherly.Messaging.Outboxing.BackgroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessageJob(
    IOutboxRepository outboxRepository,
    ILogger<ProcessOutboxMessageJob> logger,
    IUnitOfWork unitOfWork) 
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var messages = await outboxRepository.GetUnprocessedAsync(10);
            if (messages.Any() is false) return;
            
            foreach (var message in messages)
            {
                // Process message
                await outboxRepository.MarkAsProcessedAsync(message);
            }
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "Error processing outbox message");
        }
    }
}