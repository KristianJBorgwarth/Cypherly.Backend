using Cypherly.Persistence.ModelConfigurations;
using Cypherly.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Persistence.Context;

public abstract class CypherlyBaseDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessage { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageModelConfiguration());
    }
}