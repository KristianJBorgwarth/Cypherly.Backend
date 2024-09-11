using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Persistence.ModelConfigurations;
using Cypherly.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Authentication.Persistence.Context;

public class AuthenticationDbContext(DbContextOptions options) : CypherlyBaseDbContext(options)
{
    public DbSet<User> User { get; set; } = null!;
    public DbSet<VerificationCode> VerificationCode { get; set; } = null!;
    
    public DbSet<Claim> Claim { get; set; } = null!;
    
    public DbSet<UserClaim> UserClaim { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
        modelBuilder.ApplyConfiguration(new VerificationCodeModelConfiguration());
        modelBuilder.ApplyConfiguration(new ClaimModelConfiguration());
        modelBuilder.ApplyConfiguration(new UserClaimModelConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}