using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;

public class GetFriendRequestsQueryHandler(
    IUserProfileRepository userProfileRepository,
    ILogger<GetFriendRequestsQueryHandler> logger)
    : IQueryHandler<GetFriendRequestsQuery, GetFriendRequestsDto>
{
    public Task<Result<GetFriendRequestsDto>> Handle(GetFriendRequestsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}