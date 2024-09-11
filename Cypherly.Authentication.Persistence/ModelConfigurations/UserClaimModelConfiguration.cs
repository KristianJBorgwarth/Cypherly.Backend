using Cypherly.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.Authentication.Persistence.ModelConfigurations;

public class UserClaimModelConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("UserClaim");
        
        builder.HasKey(e => new { e.UserId, e.ClaimId });
        
        builder.HasOne(uc => uc.User)
            .WithMany(u => u.UserClaims)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uc => uc.Claim)
            .WithMany(c => c.UserClaims)
            .HasForeignKey(uc => uc.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}