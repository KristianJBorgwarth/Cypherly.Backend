using Cypherly.Authentication.Domain.Entities;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.Login;

public sealed record LoginDto
{
    public Guid Id { get; init; }
    public bool IsVerified { get; init; }
    public Guid? DeviceId { get; init; }

    private LoginDto() { } // Hide the constructor to force the use of the Map method

    public static LoginDto Map(Domain.Aggregates.User user, bool isVerified, Device? device = null)
    {
        return new LoginDto()
        {
            Id = user.Id,
            DeviceId = device?.Id,
            IsVerified = isVerified
        };
    }
}