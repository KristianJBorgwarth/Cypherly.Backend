using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Authentication.Persistence.Repositories;

public class UserRepository(AuthenticationDbContext context) : IUserRepository
{
    public async Task CreateAsync(User entity)
    {
        await context.User.AddAsync(entity);
    }

    public Task DeleteAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.User.FindAsync(id);
    }

    public Task UpdateAsync(User entity)
    {
        context.User.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.User.FirstOrDefaultAsync(c => c.Email.Address.Equals(email));
    }
}