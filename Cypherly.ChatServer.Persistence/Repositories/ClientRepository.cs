using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Domain.Aggregates;
using Cypherly.ChatServer.Persistence.Context;

namespace Cypherly.ChatServer.Persistence.Repositories;

public class ClientRepository(ChatServerDbContext context) : IClientRepository
{
    public async Task CreateAsync(Client entity)
    {
        await context.Client.AddAsync(entity);
    }

    public Task DeleteAsync(Client entity)
    {
        throw new NotImplementedException();
    }

    public Task<Client?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Client entity)
    {
        throw new NotImplementedException();
    }
}