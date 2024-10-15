using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.BlockUser;

public class BlockUserCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUserProfileService userProfileService,
    IUnitOfWork uow,
    ILogger<BlockUserCommandHandler> logger)
    : ICommandHandler<BlockUserCommand>
{
    public async Task<Result> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserId);
            if (userProfile is null)
            {
                logger.LogError("User profile not found for user id {UserId}", request.UserId);
                return Result.Fail(Errors.General.NotFound(request.UserId));
            }

            var blockedUserProfile = await userProfileRepository.GetByUserTag(request.BlockedUserTag);
            if (blockedUserProfile is null)
            {
                logger.LogError("User profile not found for user id {UserId}", request.BlockedUserTag);
                return Result.Fail(Errors.General.NotFound(request.BlockedUserTag));
            }

            userProfileService.BlockUser(userProfile, blockedUserProfile);
            await uow.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error blocking user {UserId} for user {BlockedUserTag}", request.UserId,
                request.BlockedUserTag);
            return Result.Fail(Errors.General.UnspecifiedError(
                "Exception occured whilte attempting to block user. Please Check logs for more details"));
        }
    }
}