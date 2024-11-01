namespace Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;

public sealed class UserDeletedMessage : BaseMessage
{
    public Guid UserId { get; init; }
    public string Email { get; init; }

    public UserDeletedMessage(Guid userId, string email, Guid correlationId, Guid? causationId = null) : base(correlationId, causationId)
    {
        UserId = userId;
        Email = email;
    }
}