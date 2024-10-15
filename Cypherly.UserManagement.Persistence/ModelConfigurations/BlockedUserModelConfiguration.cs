using Cypherly.UserManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.UserManagement.Persistence.ModelConfigurations;

public class BlockedUserModelConfiguration : IEntityTypeConfiguration<BlockedUser>
{
    public void Configure(EntityTypeBuilder<BlockedUser> builder)
    {
        builder.ToTable("BlockedUser");
        
        builder.HasKey(e => new { UserId = e.BlockingUserProfileId, BlockedUserId = e.BlockedUserProfileId });
        
        builder.Property(e => e.BlockingUserProfileId)
            .IsRequired();
    }
}