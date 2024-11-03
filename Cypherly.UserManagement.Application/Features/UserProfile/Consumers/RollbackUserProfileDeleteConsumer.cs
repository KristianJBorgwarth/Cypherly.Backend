using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Consumers;

public class RollbackUserProfileDeleteConsumer(
    IUserProfileRepository userProfileRepository,
    IUserProfileLifecycleService userProfileLifecycleService,
    IUnitOfWork unitOfWork,
    ILogger<RollbackUserProfileDeleteConsumer> logger) : IConsumer<UserDeleteFailedMessage>
{
    public async Task Consume(ConsumeContext<UserDeleteFailedMessage> context)
    {
        try
        {
            var message = context.Message;

            if (!message.ContainsService(ServiceType.UserManagementService)) return;

            var user = await userProfileRepository.GetByIdAsync(message.UserId);
            if (user is null)
            {
                logger.LogError("User with id {UserId} not found", message.UserId);
                return;
            }

            logger.LogInformation("Reverting soft delete for UserProfile with id {UserId}", message.UserId);
            userProfileLifecycleService.RevertSoftDelete(user);
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An Exception occured while attempting to delete a user profile");
            throw;
        }
    }
}