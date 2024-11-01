using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Consumers;

public class UserDeleteFailedConsumer(
    IUserRepository userRepository,
    IUserService userService,
    IUnitOfWork unitOfWork,
    ILogger<UserDeleteFailedConsumer> logger)
    : IConsumer<UserDeleteFailedMessage>
{

    public async Task Consume(ConsumeContext<UserDeleteFailedMessage> context)
    {
        try
        {
            var message = context.Message;

            if (!message.ContainsService(ServiceType.AuthenticationService)) return;

            var user = await userRepository.GetByIdAsync(message.UserId);
            if (user is null)
            {
                logger.LogError("User with id {UserId} not found", message.UserId);
                return;
            }

            logger.LogInformation("Reverting soft delete for user with id {UserId}", message.UserId);
            userService.RevertSoftDelete(user);
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error consuming UserDeleteFailed message");
            throw;
        }
    }
}