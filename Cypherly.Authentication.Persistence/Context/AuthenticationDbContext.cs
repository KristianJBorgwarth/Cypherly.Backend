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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
        modelBuilder.ApplyConfiguration(new VerificationCodeModelConfiguration());
    }
}