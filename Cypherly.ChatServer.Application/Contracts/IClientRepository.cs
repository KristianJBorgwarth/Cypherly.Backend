using Cypherly.Application.Contracts.Repository;
using Cypherly.ChatServer.Domain.Aggregates;

namespace Cypherly.ChatServer.Application.Contracts;

public interface IClientRepository : IRepository<Client>
{
    Task QueryAsync(Guid id, Func<IQueryable<Client>, IQueryable<Client>>? includeFunc);
}