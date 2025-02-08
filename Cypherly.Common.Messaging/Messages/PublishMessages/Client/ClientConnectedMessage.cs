namespace Cypherly.Common.Messaging.Messages.PublishMessages.Client;

public sealed class ClientConnectedMessage : BaseMessage
{
    public Guid DeviceId { get; private set; }

    public ClientConnectedMessage(Guid deviceId, Guid correlationId, Guid? causationId = null) : base(correlationId, causationId)
    {
        DeviceId = deviceId;
    }
}