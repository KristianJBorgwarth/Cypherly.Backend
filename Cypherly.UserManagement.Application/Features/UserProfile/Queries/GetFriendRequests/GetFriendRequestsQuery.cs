using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;

public sealed record GetFriendRequestsQuery : IQuery<List<GetFriendRequestsDto>>
{
    public required Guid UserId { get; init; }
}