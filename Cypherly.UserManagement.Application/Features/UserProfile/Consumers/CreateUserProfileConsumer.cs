using Cypherly.Application.Contracts.Repository;
using Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Consumers;

public class CreateUserProfileConsumer(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IUserProfileLifecycleService userProfileLifecycleService,
    ILogger<CreateUserProfileConsumer> logger)
    : IConsumer<CreateUserProfileRequest>
{
    public async Task Consume(ConsumeContext<CreateUserProfileRequest> context)
    {
        try
        {
            var message = context.Message;

            var profile = userProfileLifecycleService.CreateUserProfile(message.UserId, message.Username);

            await userProfileRepository.CreateAsync(profile);

            await unitOfWork.SaveChangesAsync();

            await context.RespondAsync(new CreateUserProfileResponse());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occured while attempting to create a user profile");
            await context.RespondAsync(new CreateUserProfileResponse(false, Errors.General.UnspecifiedError("An Exception occured while attempting to create a user profile").Message));
            throw;
        }
    }
}