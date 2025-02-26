// ReSharper disable ConvertToPrimaryConstructor
namespace Cypherly.Common.Messaging.Messages.PublishMessages.UserProfile;

public sealed class ProfilePictureUpdatedMessage : BaseMessage
{
    public List<Guid> ConnectionIds { get; private set; }
    public Guid UserProfileId { get; private set; }
    public string ProfilePictureUrl { get; private set; }

    public ProfilePictureUpdatedMessage(Guid userProfileId, string profilePictureUrl, List<Guid> connectionIds, Guid correlationId, Guid? causationId = null) : base(correlationId, causationId)
    {
        UserProfileId = userProfileId;
        ProfilePictureUrl = profilePictureUrl;
        ConnectionIds = connectionIds;
    }
}