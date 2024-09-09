using Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Consumers;

public class CreateUserProfileConsumer(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IUserProfileService userProfileService,
    ILogger<CreateUserProfileConsumer> logger)
    : IConsumer<CreateUserProfileRequest>
{
    public async Task Consume(ConsumeContext<CreateUserProfileRequest> context)
    {
        try
        {
            var message = context.Message;

            var profile = userProfileService.CreateUserProfile(message.UserId, message.Username);

            await userProfileRepository.CreateAsync(profile);

            await unitOfWork.SaveChangesAsync();

            await context.RespondAsync(new CreateUserProfileResponse());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occured while attempting to create a user profile");
            await context.RespondAsync(new CreateUserProfileResponse(false, Errors.General.UnspecifiedError("An Exception occured while attempting to create a user profile")));
            throw;
        }
    }
}