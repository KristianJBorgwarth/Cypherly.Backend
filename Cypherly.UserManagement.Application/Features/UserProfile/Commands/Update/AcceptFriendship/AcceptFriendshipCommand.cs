using Cypherly.Application.Abstractions;
using ICommand = Cypherly.Application.Abstractions.ICommand;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

public sealed record AcceptFriendshipCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required string FriendTag { get; init; }
}