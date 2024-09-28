﻿using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.Authentication.Persistence.ModelConfigurations;

public class UserModelConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(u => u.Id);
        builder.OwnsOne(u=> u.Email, email =>
        {
            email.Property(e => e.Address)
                .HasColumnName("Email")
                .IsRequired()
                .HasColumnType("citext") // Ensure case-insensitive to prevent duplicate emails
                .HasMaxLength(255);

            email.HasIndex(e => e.Address).IsUnique();
        });
        builder.OwnsOne(u=> u.Password, pw =>
        {
            pw.Property(p => p.HashedPassword)
                .HasColumnName("Password")
                .IsRequired()
                .HasMaxLength(255);
        });
        builder.Property(u => u.IsVerified).IsRequired();


        builder.HasMany(u => u.VerificationCodes)
            .WithOne()
            .HasForeignKey(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt=> rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}