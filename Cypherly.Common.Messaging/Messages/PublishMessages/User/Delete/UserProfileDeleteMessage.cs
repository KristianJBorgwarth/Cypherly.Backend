namespace Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;

public sealed class UserProfileDeleteMessage : BaseMessage
{
    public Guid UserProfileId { get; private set; }

    public UserProfileDeleteMessage(Guid userProfileId, Guid correlationId, Guid? causationId = null) : base(correlationId, causationId)
    {
        UserProfileId = userProfileId;
    }
}