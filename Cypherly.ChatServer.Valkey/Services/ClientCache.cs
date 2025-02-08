using System.Text.Json;
using Cypherly.ChatServer.Application.Cache.Client;
using Cypherly.ChatServer.Application.Contracts;

namespace Cypherly.ChatServer.Valkey.Services;

public class ClientCache(IValkeyCacheService valkeyCacheService) : IClientCache
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
    {
        Converters = { new ClientCacheDtoJsonConverter() },
    };

    /// <summary>
    /// Add a Client to the cache by Clients ConnectionId
    /// Adds the inverse mapping of ConnectionId to TransientId as well
    /// </summary>
    /// <param name="value">Client to be added</param>
    /// <param name="cancellationToken"></param>
    public async Task AddAsync(ClientCacheDto value, CancellationToken cancellationToken)
    {
        await valkeyCacheService.SetAsync(value.ConnectionId.ToString(), value, cancellationToken, shouldExipire: false);
        await valkeyCacheService.SetAsync(value.TransientId, value.ConnectionId, cancellationToken, shouldExipire: false);
    }
    public async Task<ClientCacheDto?> GetByTransientIdAsync(string transientId, CancellationToken cancellationToken)
    {
        var connectionIdStr = await valkeyCacheService.GetAsync<string>(transientId, _options ,cancellationToken);

        if (connectionIdStr is not null && Guid.TryParse(connectionIdStr, out var connectionId))
        {
            return await GetAsync(connectionId, cancellationToken);
        }
        return null;
    }

    /// <summary>
    /// Get a client from the cache by Clients ConnectionId
    /// </summary>
    /// <param name="id">ConnectionID for Client <see cref="ClientCacheDto"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ClientCacheDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return valkeyCacheService.GetAsync<ClientCacheDto>(id.ToString(), _options, cancellationToken);
    }

    /// <summary>
    /// Remove a client from the cache by Clients ConnectionId
    /// </summary>
    /// <param name="id">ConnectionId for Client <see cref="ClientCacheDto"/></param>
    /// <param name="cancellationToken"></param>
    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        var client = await GetAsync(id, cancellationToken);
        await valkeyCacheService.RemoveAsync(id.ToString(), cancellationToken);
        await RemoveTransientMappingAsync(client.TransientId, cancellationToken);

    }

    private async Task RemoveTransientMappingAsync(string transientId, CancellationToken cancellationToken)
    {
        await valkeyCacheService.RemoveAsync(transientId, cancellationToken);
    }
}