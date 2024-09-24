using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;

public sealed class DeleteFriendshipCommandHandler(
    IUserProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    IUserProfileService userProfileService,
    ILogger<DeleteFriendshipCommandHandler> logger)
    : ICommandHandler<DeleteFriendshipCommand>
{
    public async Task<Result> Handle(DeleteFriendshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await profileRepository.GetByIdAsync(request.UserProfileId);
            if (userProfile is null)
            {
                logger.LogError("UserProfile with id {UserProfileId} not found", request.UserProfileId);
                return Result.Fail(Errors.General.NotFound(nameof(request.UserProfileId)));
            }
            var deleteResult = userProfileService.DeleteFriendship(userProfile, request.FriendTag);
            if (!deleteResult.Success)
            {
                logger.LogError("Failed to delete friendship with FriendTag {FriendTag} for UserProfileId {UserProfileId}", request.FriendTag, request.UserProfileId);
                return Result.Fail(deleteResult.Error);
            }

            await profileRepository.UpdateAsync(userProfile);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occurred in DeleteFriendshipCommandHandler for command with UserProfileId {UserProfileId} and FriendTag {FriendTag}", request.UserProfileId, request.FriendTag);
            return Result.Fail(Errors.General.UnspecifiedError("An exception occured while attempting to delete a friendship."));
        }
    }
}