using Cypherly.Application.Contracts.Messaging.PublishMessages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.Messaging.MassTransit;

public class Producer<TMessage>(
    IPublishEndpoint publishEndpoint,
    ILogger<Producer<TMessage>> logger)
    : IProducer<TMessage>
    where TMessage : BaseMessage
{

    public async Task PublishMessageAsync(TMessage message)
    {
        try
        {
            await publishEndpoint.Publish(message);
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