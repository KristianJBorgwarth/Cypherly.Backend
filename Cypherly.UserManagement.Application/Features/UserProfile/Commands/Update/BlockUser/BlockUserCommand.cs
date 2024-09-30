using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.BlockUser;

public sealed record BlockUserCommand : ICommand
{
    public Guid UserId { get; init; }
    public required string BlockedUserTag { get; init; }
}