using Cypherly.Persistence.Context;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Persistence.ModelConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.UserManagement.Persistence.Context;

public class UserManagementDbContext(DbContextOptions options) : CypherlyBaseDbContext(options)
{
    public DbSet<UserProfile> UserProfile { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserProfileModelConfiguration());
    }
}