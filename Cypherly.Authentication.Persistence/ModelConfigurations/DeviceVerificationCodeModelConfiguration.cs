using Cypherly.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.Authentication.Persistence.ModelConfigurations;

public class DeviceVerificationCodeModelConfiguration : IEntityTypeConfiguration<DeviceVerificationCode>
{
    public void Configure(EntityTypeBuilder<DeviceVerificationCode> builder)
    {
        builder.ToTable("DeviceVerificationCode");

        builder.HasKey(vc => vc.Id);

        builder.Property(vc => vc.Id)
            .ValueGeneratedNever();

        builder.OwnsOne(dv => dv.Code, vc =>
        {
            vc.Property(v => v.Value)
                .HasColumnName("Code")
                .HasMaxLength(20)
                .IsRequired();

            vc.Property(v => v.ExpirationDate)
                .IsRequired();

            vc.Property(v => v.IsUsed)
                .IsRequired();

            vc.HasIndex(v => v.Value);

            vc.HasIndex(v => v.ExpirationDate);
        });

        builder.HasOne<Device>()
            .WithMany(d => d.VerificationCodes)
            .HasForeignKey(d => d.DeviceId);

        builder.HasIndex(vc => vc.DeviceId);
    }
}