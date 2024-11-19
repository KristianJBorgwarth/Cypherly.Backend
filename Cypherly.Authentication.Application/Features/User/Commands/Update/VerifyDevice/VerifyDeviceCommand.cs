using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.VerifyDevice;

public sealed record VerifyDeviceCommand : ICommand
{
    public required Guid UserId { get; init; }
    public required Guid DeviceId { get; init; }
    public required string DeviceVerificationCode { get; init; }
}