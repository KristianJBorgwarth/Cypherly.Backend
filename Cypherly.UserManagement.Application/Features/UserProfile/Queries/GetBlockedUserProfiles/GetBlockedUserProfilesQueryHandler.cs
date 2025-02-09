using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;

public sealed class GetBlockedUserProfilesQueryHandler : IQueryHandler<GetBlockedUserProfilesQuery, List<GetBlockedUserProfilesDto>>
{
    public Task<Result<List<GetBlockedUserProfilesDto>>> Handle(GetBlockedUserProfilesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}