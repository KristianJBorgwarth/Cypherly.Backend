using Cypherly.Application.Contracts.Cache;
using Cypherly.ChatServer.Application.Cache.Client;

namespace Cypherly.ChatServer.Application.Contracts;

public interface IClientCache : ICache<ClientCacheDto>
{
    Task<ClientCacheDto?> GetByTransientIdAsync(string transientId, CancellationToken cancellationToken);
}