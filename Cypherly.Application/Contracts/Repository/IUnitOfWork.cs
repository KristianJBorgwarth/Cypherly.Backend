using Microsoft.EntityFrameworkCore;

namespace Cypherly.Application.Contracts.Repository;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}