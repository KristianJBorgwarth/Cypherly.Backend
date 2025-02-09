namespace Cypherly.ChatServer.Application.Features.ChangeEvent.ProfilePictureUpdated;

public sealed class ProfilePictureUpdatedChangeEvent(Guid userProfileId, string profilePictureUrl)
{
    public Guid UserProfileId { get; private set; } = userProfileId;
    public string ProfilePictureUrl { get; private set; } = profilePictureUrl;
}