using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IAuthenticationService
{
    RefreshToken GenerateRefreshToken(Aggregates.User user, Guid deviceId);
    bool VerifyRefreshToken(Aggregates.User user, Guid deviceId, string refreshToken);
    void GenerateLoginVerificationCode(Aggregates.User user);
    void Logout(Aggregates.User user, Guid deviceId);
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

    //TODO: test this
    public void GenerateLoginVerificationCode(Aggregates.User user)
    {
        user.AddVerificationCode(UserVerificationCodeType.Login);
        user.AddDomainEvent(new VerificationCodeGeneratedEvent(user.Id, UserVerificationCodeType.Login));
    }

    //TODO: write tests for this
    public void Logout(Aggregates.User user, Guid deviceId)
    {
        var device = user.GetDevice(deviceId);

        device.RevokeRefreshTokens();
        device.SetLastSeen();
        device.SetDelete();
    }
}