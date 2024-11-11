using Cypherly.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.Authentication.Persistence.ModelConfigurations;

public class DeviceModelConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Device");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever();

        builder.Property(d => d.Name)
            .IsRequired();

        builder.Property(d => d.AppVersion)
            .IsRequired();

        builder.Property(d => d.PublicKey)
            .IsRequired();

        builder.Property(d=> d.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(d => d.Type)
            .HasConversion<string>();

        builder.Property(d => d.Platform)
            .HasConversion<string>();

        builder.HasIndex(d=> d.UserId);
    }
}