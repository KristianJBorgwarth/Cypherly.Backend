using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;

public sealed record CreateFriendshipCommand : ICommand
{
    public Guid UserId { get; init; }
    public required string FriendTag { get; init; }
}