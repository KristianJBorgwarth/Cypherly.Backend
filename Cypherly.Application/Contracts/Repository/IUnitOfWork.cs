using Microsoft.EntityFrameworkCore;

namespace Cypherly.Application.Contracts.Repository;

public interface IUnitOfWork<TContext>
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}