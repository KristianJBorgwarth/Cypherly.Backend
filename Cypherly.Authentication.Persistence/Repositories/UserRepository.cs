using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Aggregates;

namespace Cypherly.Authentication.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    public Task CreateAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User entity)
    {
        throw new NotImplementedException();
    }
}