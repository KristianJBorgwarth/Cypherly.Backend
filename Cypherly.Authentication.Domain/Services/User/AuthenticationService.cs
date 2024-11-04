using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IAuthenticationService
{
    RefreshToken GenerateRefreshToken(Aggregates.User user);
}
public class AuthenticationService : IAuthenticationService
{
    public RefreshToken GenerateRefreshToken(Aggregates.User user)
    {
        user.AddRefreshToken();
        return user.GetActiveRefreshToken()!;
    }
}