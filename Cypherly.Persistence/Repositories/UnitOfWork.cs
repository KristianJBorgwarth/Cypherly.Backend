using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Cypherly.Domain.Common;
using Cypherly.Persistence.Outbox;
using Cypherly.Application.Contracts.Repository;

namespace Cypherly.Persistence.Repositories;

public class UnitOfWork<TContext>(TContext context) : IUnitOfWork where TContext : DbContext
{
    /// <summary>
    /// Saves all changes made in the context to the database.
    /// </summary>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDomainEventsToOutboxMessages();
        UpdateAuditableEntities();
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Converts domain events of all <see cref="AggregateRoot"/> entities tracked by the context into
    /// outbox messages and persists them to the database.
    /// </summary>
    private void ConvertDomainEventsToOutboxMessages()
    {
        var domainEntities = context.ChangeTracker.Entries<AggregateRoot>()
            .Select(x => x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.DomainEvents;
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            }).Select(domainEvent => new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccurredOn = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(domainEvent,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }),
            }).ToList();

        context.Set<OutboxMessage>().AddRange(domainEntities);
    }

    /// <summary>
    /// Updates the Created and LastModified timestamps for auditable entities.
    /// </summary>
    private void UpdateAuditableEntities()
    {
        var entities = context.ChangeTracker.Entries<Entity>().Where(e=> e.State is EntityState.Added or EntityState.Modified);

        foreach (var entity in entities)
        {
            if (entity.State is EntityState.Added)
            {
                entity.Entity.SetCreated();
            }

            entity.Entity.SetLastModified();
        }
    }
}