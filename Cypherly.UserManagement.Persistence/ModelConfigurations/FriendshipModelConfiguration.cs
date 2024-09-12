using Cypherly.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.UserManagement.Persistence.ModelConfigurations;

public class FriendshipModelConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("Friendship");

        builder.HasKey(e => new { UserId = e.UserProfileId, FriendId = e.FriendProfileId });

        builder.Property(e => e.Status)
            .IsRequired();
    }
}