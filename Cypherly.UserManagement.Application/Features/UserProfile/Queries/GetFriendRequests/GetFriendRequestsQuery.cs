using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;

public sealed class GetFriendRequestsQuery : IQuery<GetFriendRequestsDto>
{
    public required Guid UserId { get; init; }
}