using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Enums;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using MassTransit;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Consumers;

public class DeleteUserProfileConsumer(
    IUserProfileRepository userProfileRepository,
    IUserProfileService userProfileService,
    IUnitOfWork unitOfWork,
    IProducer<OperationSuccededMessage> producer)
    : IConsumer<UserProfileDeleteMessage>
{
    public async Task Consume(ConsumeContext<UserProfileDeleteMessage> context)
    {
        var message = context.Message;
        var user = await userProfileRepository.GetByIdAsync(message.UserProfileId);

        if (user is null)
        {
            throw new KeyNotFoundException($"User with id {message.UserProfileId} not found.");
        }

        userProfileService.SoftDelete(user);
        await unitOfWork.SaveChangesAsync();
        await producer.PublishMessageAsync(new OperationSuccededMessage(OperationType.UserProfileDelete, message.CorrelationId, message.Id));
    }
}