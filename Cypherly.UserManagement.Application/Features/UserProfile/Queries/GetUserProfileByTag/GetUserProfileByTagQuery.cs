using Cypherly.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public sealed record GetUserProfileByTagQuery : IQuery<GetUserProfileByTagDto>
{
    public required Guid Id { get; init; }
    public required string Tag { get; init; }
}