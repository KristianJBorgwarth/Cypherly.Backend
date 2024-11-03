namespace Cypherly.Common.Messaging.Messages.PublishMessages;

public interface IProducer<in TMessage> where TMessage : BaseMessage
{
    Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken = default);
}