namespace Cypherly.Application.Contracts.Messaging.PublishMessages;

public interface IProducer<in TMessage> where TMessage : BaseMessage
{
    Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken = default);
}