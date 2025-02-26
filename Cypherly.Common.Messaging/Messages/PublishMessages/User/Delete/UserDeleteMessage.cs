namespace Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;

public sealed class UserDeleteMessage : BaseMessage
{
    public Guid UserProfileId { get; private set; }

    public UserDeleteMessage(Guid userProfileId, Guid correlationId, Guid? causationId = null) : base(correlationId, causationId)
    {
        UserProfileId = userProfileId;
    }
}