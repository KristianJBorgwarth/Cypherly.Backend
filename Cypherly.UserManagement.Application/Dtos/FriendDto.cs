namespace Cypherly.UserManagement.Application.Dtos;

public sealed record FriendDto
{
    public required string Username { get; init; }
    public required string UserTag { get; init; }
    public string? DisplayName { get; init; }
    public string? ProfilePictureUrl { get; init; }
}