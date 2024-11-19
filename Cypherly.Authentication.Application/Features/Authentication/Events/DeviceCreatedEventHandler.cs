using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Email;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.Authentication.Events;

public class DeviceCreatedEventHandler(
    IUserRepository userRepository,
    IProducer<SendEmailMessage> emailProducer,
    ILogger<DeviceCreatedEventHandler> logger)
    : IDomainEventHandler<DeviceCreatedEvent>
{
    public async Task Handle(DeviceCreatedEvent notification, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(notification.UserId);

        if (user is null)
        {
            logger.LogError("User with id {UserId} not found", notification.UserId);
            throw new InvalidOperationException("User not found");
        }

        var userDevices = user.GetValidDevices();

        if (userDevices.Count > 1)
        {
            //Proceed to notify all devices of the newly registered device
        }
        else
        {
            var emailMessage = new SendEmailMessage(user.Email.Address, "Cypherly: Device Registration", $"A new device has been registered to your account. If this was not you, please contact support immediately. Here is your device verification code: {notification.DeviceVerificationCode}" , Guid.NewGuid());
            await emailProducer.PublishMessageAsync(emailMessage, cancellationToken);
        }

    }
}