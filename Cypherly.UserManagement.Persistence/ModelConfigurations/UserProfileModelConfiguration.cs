using Cypherly.UserManagement.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.UserManagement.Persistence.ModelConfigurations;

public class UserProfileModelConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        throw new NotImplementedException();
    }
}