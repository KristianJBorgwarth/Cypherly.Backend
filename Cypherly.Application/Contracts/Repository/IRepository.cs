using Cypherly.Domain.Common;

namespace Cypherly.Application.Contracts.Repository;

public interface IRepository<T> where T : AggregateRoot
{
    Task CreateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task UpdateAsync(T entity);
}