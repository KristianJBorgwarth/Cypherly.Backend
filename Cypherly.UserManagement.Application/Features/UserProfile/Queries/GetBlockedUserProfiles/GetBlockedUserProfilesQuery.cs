using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;

public sealed record GetBlockedUserProfilesQuery : IQuery<List<GetBlockedUserProfilesDto>>
{
    public required Guid UserId { get; init; }
}