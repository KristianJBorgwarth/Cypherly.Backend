using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Application.Features.Authentication.Token;

public interface IJwtService
{
    string GenerateToken(Guid UserId, Guid DeviceId, List<UserClaim> userClaims);
}