using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;

public sealed record DeleteFriendshipCommand : ICommand
{
    public required Guid Id { get; init; }
    public required string FriendTag { get; init; }
}