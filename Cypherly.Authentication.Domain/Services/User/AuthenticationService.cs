using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IAuthenticationService
{
    RefreshToken GenerateRefreshToken(Aggregates.User user, Guid deviceId);
}
public class AuthenticationService : IAuthenticationService
{
    public RefreshToken GenerateRefreshToken(Aggregates.User user, Guid deviceId)
    {
        var device = user.GetDevice(deviceId);
        device.AddRefreshToken();
        return device.GetActiveRefreshToken()!;
    }
}