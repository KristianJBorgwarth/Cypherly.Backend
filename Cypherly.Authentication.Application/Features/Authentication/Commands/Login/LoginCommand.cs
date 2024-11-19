using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Domain.Enums;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.Login;

public sealed record LoginCommand : ICommand<LoginDto>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string DeviceName { get; init; }
    public required string Base64DevicePublicKey { get; init; }
    public required string DeviceAppVersion { get; init; }
    public required DeviceType DeviceType { get; init; }
    public required DevicePlatform DevicePlatform { get; init; }
}