using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.Logout;

public class LogoutCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required Guid DeviceId { get; init; }
}