using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Features.User.Queries.GetUserDevices;

namespace Cypherly.Authentication.Application.Features.User.Queries.GetUserDevices;

public sealed record GetUserDevicesQuery : IQuery<GetUserDevicesDto>
{
    public required Guid UserId { get; init; }
}