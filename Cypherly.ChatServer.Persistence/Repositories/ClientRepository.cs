using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Domain.Aggregates;
using Cypherly.ChatServer.Persistence.Context;

namespace Cypherly.ChatServer.Persistence.Repositories;

public class ClientRepository(ChatServerDbContext context) : IClientRepository
{
    public Task CreateAsync(Client entity)
    {
        throw new NotImplementedException();
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