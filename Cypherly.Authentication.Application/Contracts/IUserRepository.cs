using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Domain.Aggregates;

namespace Cypherly.Authentication.Application.Contracts;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByEmail(string email);
}