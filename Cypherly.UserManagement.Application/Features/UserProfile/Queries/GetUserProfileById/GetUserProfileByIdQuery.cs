using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileById;

public sealed record GetUserProfileByIdQuery : IQuery<GetUserProfileByIdDto>
{
    public Guid UserProfileId { get; init; }
}