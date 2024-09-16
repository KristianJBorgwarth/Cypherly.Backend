using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

public class AcceptFriendshipCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUserProfileService userProfileService,
    IUnitOfWork unitOfWork,
    ILogger<AcceptFriendshipCommandHandler> logger)
    : ICommandHandler<AcceptFriendshipCommand>
{
    public async Task<Result> Handle(AcceptFriendshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserId);
            if(userProfile is null)
            {
                logger.LogWarning("User not found: {UserId}", request.UserId);
                return Result.Fail(Errors.General.NotFound(request.UserId));
            }

            var result = userProfileService.AcceptFriendship(userProfile, request.FriendTag);
            if(result.Success is false)
            {
                logger.LogWarning("Error accepting friendship: {Error} {UserId} {FriendTag}",result.Error, request.UserId, request.FriendTag);
                return Result.Fail(result.Error);
            }

            await userProfileRepository.UpdateAsync(userProfile);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error accepting friendship: {UserId} {FriendTag}", request.UserId, request.FriendTag);
            return Result.Fail(Errors.General.UnspecifiedError("An exception occured while accepting friendship"));
        }
    }
}