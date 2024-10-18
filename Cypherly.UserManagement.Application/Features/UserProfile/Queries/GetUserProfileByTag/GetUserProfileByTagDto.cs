namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public sealed record GetUserProfileByTagDto
{
    public required string Username { get; init; }
    public required string UserTag { get; init; }
    public string? DisplayName { get; init; }
    public string? ProfilePictureUrl { get; init; }
}