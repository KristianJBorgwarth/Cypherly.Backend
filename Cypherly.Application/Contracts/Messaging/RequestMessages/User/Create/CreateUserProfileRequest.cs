namespace Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;

public sealed class CreateUserProfileRequest(Guid userId, string username) : RequestMessage
{
    public Guid UserId { get; init; } = userId;
    public required string Username { get; init; } = username;
}