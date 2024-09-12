using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;

public class CreateFriendshipCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUserProfileService userProfileService,
    IUnitOfWork unitOfWork,
    ILogger<CreateFriendshipCommandHandler> logger)
    : ICommandHandler<CreateFriendshipCommand>
{
    public async Task<Result> Handle(CreateFriendshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var friend = await userProfileRepository.GetByUserTag(request.FriendTag);
            if (friend is null)
            {
                logger.LogWarning("Friend not found for {FriendTag}", request.FriendTag);
                return Result.Fail(Errors.General.NotFound("Friend not found"));
            }

            var userProfile = await userProfileRepository.GetByIdAsync(request.UserId);
            if (userProfile is null)
            {
                logger.LogWarning("User not found for {UserId}", request.UserId);
                return Result.Fail(Errors.General.NotFound("User not found"));
            }

            var result = userProfileService.CreateFriendship(userProfile, friend);
            if (result.Success is false)
            {
                logger.LogWarning("Error creating friendship between {UserId} and {FriendId}", request.UserId, friend.Id);
                return Result.Fail(result.Error);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Command}", nameof(CreateFriendshipCommand));
            return Result.Fail(Errors.General.UnspecifiedError("Exception occured while attempting to create friendship"));
        }
    }
}