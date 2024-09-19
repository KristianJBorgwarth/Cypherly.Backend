using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Application.Services.Authentication;

public interface IJwtService
{
    string GenerateToken(Guid UserId, string UserEmail, List<UserClaim> claims);
}