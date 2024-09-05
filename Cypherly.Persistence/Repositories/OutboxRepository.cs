using Cypherly.Persistence.Context;
using Cypherly.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Persistence.Repositories;

public class OutboxRepository<TContext> : IOutboxRepository where TContext : CypherlyBaseDbContext
{
    public Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize)
    {
        throw new NotImplementedException();
    }

    public Task MarkAsProcessedAsync(OutboxMessage message)
    {
        throw new NotImplementedException();
    }
}