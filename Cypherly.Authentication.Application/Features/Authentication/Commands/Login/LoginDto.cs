using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.Login;

public sealed record LoginDto
{
    public required Guid UserId { get; init; }
    public required bool IsVerified { get; init; }
    public required Guid? DeviceId { get; init; }
    public required Guid? DeviceConnectionId { get; init; }

    private LoginDto() { } // Hide the constructor to force the use of the Map method

    public static LoginDto Map(Domain.Aggregates.User user, bool isVerified, Device? device = null)
    {
        return new LoginDto()
        {
            UserId = user.Id,
            DeviceId = device?.Id,
            IsVerified = isVerified,
            DeviceConnectionId = device?.ConnectionId,
        };
    }
}