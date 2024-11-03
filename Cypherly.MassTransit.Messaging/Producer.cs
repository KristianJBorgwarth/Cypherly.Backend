using Cypherly.Common.Messaging.Messages.PublishMessages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.MassTransit.Messaging;

public class Producer<TMessage>(
    IPublishEndpoint publishEndpoint,
    ILogger<Producer<TMessage>> logger)
    : IProducer<TMessage>
    where TMessage : BaseMessage
{

    public async Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await publishEndpoint.Publish(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Producer} for message with id: {Id}",
                nameof(Producer<TMessage>),
                message.Id);
            throw;
        }
    }
}