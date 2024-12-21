using Cypherly.Authentication.Domain.Entities;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IAuthenticationService
{
    RefreshToken GenerateRefreshToken(Aggregates.User user, Guid deviceId);
    bool VerifyRefreshToken(Aggregates.User user, Guid deviceId, string refreshToken);
}
public class AuthenticationService : IAuthenticationService
{
    //TODO: test this
    public RefreshToken GenerateRefreshToken(Aggregates.User user, Guid deviceId)
    {
        var device = user.GetDevice(deviceId);
        
        var token = device.GetActiveRefreshToken();
        
        if (token is null)
        {
            var newToken = device.AddRefreshToken();
            return newToken;
        }
        
        token.Revoke();
        var refreshedToken = device.AddRefreshToken();
        return refreshedToken;
    }
    
    //TODO: test this
    public bool VerifyRefreshToken(Aggregates.User user, Guid deviceId, string refreshToken)
    {
        var device = user.GetDevice(deviceId);
        var token = device.GetActiveRefreshToken();
        return token?.Token == refreshToken;
    }
}