using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Domain.Enums;

namespace Cypherly.Authentication.Application.Features.User.Commands.Create;

public sealed record CreateUserCommand : ICommand<CreateUserDto>
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string DeviceName { get; init; }
    public required string DevicePublicKey { get; init; }
    public required string DeviceAppVersion { get; init; }
    public DeviceType DeviceType { get; init; }
    public DevicePlatform DevicePlatform { get; init; }
}