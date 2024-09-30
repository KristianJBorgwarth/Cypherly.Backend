using Cypherly.UserManagement.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.UserManagement.Persistence.ModelConfigurations;

public class UserProfileModelConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(x=> x.UserTag, y =>
        {
            y.Property(x => x.Tag)
                .HasMaxLength(58) // 50 (max username length) + 1 (hash) + 6 (number) + 1 (letter)
                .IsRequired();
        });

        builder.Property(x => x.DisplayName)
            .HasMaxLength(20);

        builder.Property(x => x.ProfilePictureUrl);

        builder.HasMany(x => x.FriendshipsInitiated)
            .WithOne(x => x.UserProfile)
            .HasForeignKey(x => x.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.FriendshipsReceived)
            .WithOne(x => x.FriendProfile)
            .HasForeignKey(x => x.FriendProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x=> x.BlockedUsers)
            .WithOne()
            .HasForeignKey(x => x.BlockingUserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}