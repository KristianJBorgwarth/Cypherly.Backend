using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Authentication.Persistence.Repositories;

public class UserRepository(AuthenticationDbContext context) : IUserRepository
{
    public async Task CreateAsync(User entity) => await context.User.AddAsync(entity);

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

    public async Task<User?> GetUserByEmail(string email) => await context.User.FirstOrDefaultAsync(c => c.Email.Address.Equals(email));
}