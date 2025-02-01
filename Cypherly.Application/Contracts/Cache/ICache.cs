namespace Cypherly.Application.Contracts.Cache;

public interface ICache<T>
{
    Task AddAsync(T value, CancellationToken cancellationToken);
    Task<T?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken);
}