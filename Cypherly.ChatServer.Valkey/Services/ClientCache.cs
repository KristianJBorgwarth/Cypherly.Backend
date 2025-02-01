using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Domain.Aggregates;

namespace Cypherly.ChatServer.Valkey.Services;

public class ClientCache(IValkeyCacheService valkeyCacheService) : IClientCache
{
    public Task AddAsync(Client value, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public Task<Client?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public Task RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}