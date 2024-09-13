namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileById;

public sealed record GetUserProfileByIdDto
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string UserTag { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public string? DisplayName { get; init; }
}

