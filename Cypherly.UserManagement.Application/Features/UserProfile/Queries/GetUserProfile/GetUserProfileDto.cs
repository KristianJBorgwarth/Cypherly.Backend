namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;

public sealed record GetUserProfileDto
{
    //TODO convert to self contained mapped dto instead of automapper, cause the shit sucks
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string UserTag { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public string? DisplayName { get; init; }
}

