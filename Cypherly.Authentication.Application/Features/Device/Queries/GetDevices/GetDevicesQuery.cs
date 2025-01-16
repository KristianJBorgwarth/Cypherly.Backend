using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.Device.Queries.GetDevices;

public sealed record GetDevicesQuery : IQuery<GetDevicesDto>
{
    public required Guid UserId { get; init; }
}