using Cypherly.ChatServer.Domain.Aggregates;
using Cypherly.ChatServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.ChatServer.Persistence.ModelConfigurations;

public class ClientModelConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Client");

        builder.HasKey(x=> x.Id);

        builder.Property(x=> x.ConnectionId)
            .IsRequired();

        builder.HasIndex(x=> x.ConnectionId)
            .IsUnique();

        // Only relevant for caching purposes in relation to SignalR connections
        builder.Ignore(x => x.TransientId);

        builder.HasMany(x=> x.ChatMessages)
            .WithOne()
            .HasForeignKey(x=> x.RecipientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}