using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.Login;

public sealed record LoginDto
{
    public Guid UserId { get; private init; }
    public bool IsVerified { get; private init; }
    public Guid? DeviceId { get; private init; }

    private LoginDto() { } // Hide the constructor to force the use of the Map method

    public static LoginDto Map(Domain.Aggregates.User user, bool isVerified, Device? device = null)
    {
        return new LoginDto()
        {
            UserId = user.Id,
            DeviceId = device?.Id,
            IsVerified = isVerified
        };
    }
}