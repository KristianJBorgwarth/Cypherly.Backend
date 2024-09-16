using ICommand = Cypherly.Application.Abstractions.ICommand;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

public sealed record AcceptFriendshipCommand : ICommand
{
    public required Guid UserId { get; init; }
    public required string FriendTag { get; init; }
}