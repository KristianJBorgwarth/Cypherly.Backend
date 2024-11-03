using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Email;
using MassTransit;
using MinimalEmail.API.Email;

namespace MinimalEmail.API.Features.Consumers;

public class SendEmailConsumer(
    IEmailService emailService,
    IProducer<OperationSuccededMessage> operationSuccededProducer,
    ILogger<SendEmailConsumer> logger)
    : IConsumer<SendEmailMessage>
{
    public async Task Consume(ConsumeContext<SendEmailMessage> context)
    {
        try
        {
            var message = context.Message;

            logger.LogInformation("Sending email to {To} with subject {Subject}",
                message.To,
                message.Subject);

            await emailService.SendEmailAsync(message.To, message.Subject, message.Body);
            await operationSuccededProducer.PublishMessageAsync(new OperationSuccededMessage(
                OperationType.SendEmail,
                message.CorrelationId,
                message.Id));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in {Consumer} for message with id: {Id}",
                nameof(SendEmailConsumer),
                context.Message.Id);
            throw;
        }
    }
}