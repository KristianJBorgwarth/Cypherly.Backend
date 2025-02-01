namespace Cypherly.Common.Messaging.Messages.RequestMessages.Client;

public sealed class CreateClientRequest(Guid DeviceId, Guid ConnectionId) : RequestMessage
{
    public Guid DeviceId { get; private set; } = DeviceId;
    public Guid ConnectionId { get; private set; } = ConnectionId;
}