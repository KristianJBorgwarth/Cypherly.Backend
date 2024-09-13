
namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;

public sealed record GetFriendsDto
{
    public required string Username { get; init; }
    public required string UserTag { get; init; }
    public string? DisplayName { get; init; }
    public string? ProfilePictureUrl { get; init; }
}