﻿using Cypherly.Authentication.Domain.Aggregates;
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

        builder.Property(vc => vc.Id)
            .ValueGeneratedNever();

        builder.Property(vc => vc.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(vc => vc.CodeType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(vc => vc.ExpirationDate)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany(u => u.VerificationCodes)
            .HasForeignKey(vc => vc.UserId);

        builder.HasIndex(vc => vc.UserId);
    }
}