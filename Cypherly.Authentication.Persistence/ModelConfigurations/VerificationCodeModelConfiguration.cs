using Cypherly.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.Authentication.Persistence.ModelConfigurations;

public class VerificationCodeModelConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.ToTable("VerificationCode");

        builder.HasKey(vc => vc.Id);

        builder.Property(vc => vc.Code)
            .HasMaxLength(6)
            .IsRequired();

        builder.Property(vc => vc.ExpirationDate)
            .IsRequired();
    }
}