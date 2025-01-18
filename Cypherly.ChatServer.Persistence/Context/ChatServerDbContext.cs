using Cypherly.ChatServer.Domain.Aggregates;
using Cypherly.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.ChatServer.Persistence.Context;

public sealed class ChatServerDbContext(DbContextOptions options) : CypherlyBaseDbContext(options)
{
    public DbSet<Client> Client { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
    }
}