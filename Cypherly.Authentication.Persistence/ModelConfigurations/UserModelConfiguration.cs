using Cypherly.Authentication.Domain.Aggregates;
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
                .HasMaxLength(255);
        });
        builder.OwnsOne(u=> u.Password, pw =>
        {
            pw.Property(p => p.HashedPassword)
                .HasColumnName("Password")
                .IsRequired()
                .HasMaxLength(255);
        });
        builder.Property(u => u.IsVerified).IsRequired();

        builder.HasOne(u => u.VerificationCode)
            .WithOne()
            .HasForeignKey<VerificationCode>(vc => vc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}