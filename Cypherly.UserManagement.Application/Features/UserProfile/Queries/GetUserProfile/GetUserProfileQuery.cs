using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;

public sealed record GetUserProfileQuery : IQuery<GetUserProfileDto>
{
    public Guid UserProfileId { get; init; }
}