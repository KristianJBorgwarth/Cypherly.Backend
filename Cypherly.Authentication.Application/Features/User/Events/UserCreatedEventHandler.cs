using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Messaging.PublishMessages;
using Cypherly.Application.Contracts.Messaging.PublishMessages.Email;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Events;

public class UserCreatedEventHandler(
    IUserRepository userRepository,
    IProducer<SendEmailMessage> emailProducer,
    ILogger<UserCreatedEventHandler> logger)
    : IDomainEventHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(notification.UserId);
        if (user is null)
        {
            logger.LogError("User with id {UserId} not found", notification.UserId);
            throw new InvalidOperationException("User not found");
        }

        var verificationCode = user.GetActiveVerificationCode(VerificationCodeType.EmailVerification);
        if (verificationCode is null)
        {
            logger.LogError("Verification code for user with id {UserId} not found", notification.UserId);
            throw new InvalidOperationException("Verification code not found");
        }

        var emailMessage = new SendEmailMessage( user.Email.Address, "Welcome to Cypherly",
            "Welcome to Cypherly! Below is your verification code: " + verificationCode.Code, Guid.NewGuid());
        await emailProducer.PublishMessageAsync(emailMessage, cancellationToken);
    }
}