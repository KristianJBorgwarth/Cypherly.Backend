using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Aggregates;

namespace Cypherly.UserManagement.Persistence.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    public Task CreateAsync(UserProfile entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<UserProfile?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UserProfile entity)
    {
        throw new NotImplementedException();
    }
}