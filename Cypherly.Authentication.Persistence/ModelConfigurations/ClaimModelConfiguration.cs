using Cypherly.Authentication.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.Authentication.Persistence.ModelConfigurations;

public class ClaimModelConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claim");

        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.ClaimType).IsRequired().HasMaxLength(30);

        builder.HasMany(e => e.UserClaims)
            .WithOne()
            .HasForeignKey(uc => uc.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}