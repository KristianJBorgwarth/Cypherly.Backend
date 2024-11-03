using Cypherly.Common.Messaging.Enums;

namespace Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;

public class UserDeleteFailedMessage : BaseMessage
{
    public Guid UserId { get; init; }
    public ServiceType[] Services { get; init; }

    public UserDeleteFailedMessage(Guid userId, Guid correlationId, Guid? causationId = null, params ServiceType[] services) : base(correlationId, causationId)
    {
        UserId = userId;
        Services = services;
    }

    public bool ContainsService(ServiceType serviceType) => Services.Contains(serviceType);
}