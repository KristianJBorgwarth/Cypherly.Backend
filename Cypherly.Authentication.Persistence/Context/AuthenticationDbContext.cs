using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Persistence.ModelConfigurations;
using Cypherly.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Authentication.Persistence.Context;

public class AuthenticationDbContext(DbContextOptions options) : CypherlyBaseDbContext(options)
{
    public DbSet<User> User { get; private set; } = null!;
    public DbSet<UserVerificationCode> VerificationCode { get; private set; } = null!;
    public DbSet<Claim> Claim { get; private set; } = null!;
    public DbSet<UserClaim> UserClaim { get; private set; } = null!;
    public DbSet<Device> Device { get; private set; } = null!;

    public DbSet<DeviceVerificationCode> DeviceVerificationCode { get; private set; } = null!;
    public DbSet<RefreshToken> RefreshToken { get; private set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceModelConfiguration());
        modelBuilder.ApplyConfiguration(new UserVerificationCodeModelConfiguration());
        modelBuilder.ApplyConfiguration(new ClaimModelConfiguration());
        modelBuilder.ApplyConfiguration(new UserClaimModelConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenModelConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceVerificationCodeModelConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}