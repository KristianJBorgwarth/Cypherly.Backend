using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Email;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Events;

public class VerificationCodeGeneratedEventHandler(
    IUserRepository userRepository,
    IProducer<SendEmailMessage> emailProducer,
    ILogger<VerificationCodeGeneratedEventHandler> logger)
    : IDomainEventHandler<VerificationCodeGeneratedEvent>
{
    public async Task Handle(VerificationCodeGeneratedEvent notification, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(notification.UserId);
        if (user is null)
        {
            logger.LogError("User with id {UserId} not found", notification.UserId);
            throw new InvalidOperationException("User not found");
        }

        var verificationCode = user.GetActiveVerificationCode(notification.CodeType);

        if (verificationCode is null)
        {
            logger.LogError("Verification code for user with id {UserId} not found", notification.UserId);
            throw new InvalidOperationException("Verification code not found");
        }

        var emailMessage = new SendEmailMessage(user.Email.Address, "Cypherly Verification", "Here is your verification code: " + verificationCode.Code, Guid.NewGuid());

        await emailProducer.PublishMessageAsync(emailMessage, cancellationToken);
    }
}