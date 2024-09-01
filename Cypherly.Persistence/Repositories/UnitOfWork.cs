using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Persistence.Repositories;

public class UnitOfWork<TContext>(TContext context) : IUnitOfWork<TContext> where TContext : DbContext
{

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        await context.SaveChangesAsync(cancellationToken);
    }
    private void ConvertDomainEventsToOutboxMessages()
    {

    }

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