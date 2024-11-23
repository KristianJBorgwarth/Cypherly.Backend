using Cypherly.Authentication.Application.Caching;

namespace Cypherly.Authentication.Application.Contracts;

public interface INonceCacheService
{
    Task AddNonceAsync(Nonce nonce, CancellationToken cancellationToken);
    Task<Nonce?> GetNonceAsync(Guid nonceId, CancellationToken cancellationToken);
    Task DeteleNonceAsync(Guid nonceId, CancellationToken cancellationToken);
}