using Cypherly.Authentication.Application.Caching.LoginNonce;

namespace Cypherly.Authentication.Application.Contracts;

public interface ILoginNonceCache
{
    Task AddNonceAsync(LoginNonce loginNonce, CancellationToken cancellationToken);
    Task<LoginNonce?> GetNonceAsync(Guid nonceId, CancellationToken cancellationToken);
    Task DeteleNonceAsync(Guid nonceId, CancellationToken cancellationToken);
}