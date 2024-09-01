using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Domain.Aggregates;

namespace Cypherly.UserManagement.Application.Contracts;

public interface IUserProfileRepository : IRepository<UserProfile>
{

}