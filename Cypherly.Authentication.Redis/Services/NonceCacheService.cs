using Cypherly.Authentication.Application.Caching;
using Cypherly.Authentication.Application.Contracts;

namespace Cypherly.Authentication.Redis.Services;

public class NonceCacheService(IRedisCacheService redisCacheService) : INonceCacheService
{
    public async Task AddNonceAsync(Nonce nonce, CancellationToken cancellationToken)
    {
        await redisCacheService.SetAsync(nonce.Id.ToString(), nonce, cancellationToken, TimeSpan.FromMinutes(5));
    }

    public async Task<Nonce?> GetNonceAsync(Guid nonceId, CancellationToken cancellationToken)
    {
        return await redisCacheService.GetAsync<Nonce>(nonceId.ToString(), cancellationToken);
    }

    public Task DeteleNonceAsync(Guid nonceId, CancellationToken cancellationToken)
    {
        return redisCacheService.RemoveAsync(nonceId.ToString(), cancellationToken);
    }
}