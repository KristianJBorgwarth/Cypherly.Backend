using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;

public sealed record GetFriendsQuery : IQuery<List<GetFriendsDto>>
{
    public required Guid UserProfileId { get; init; }
}