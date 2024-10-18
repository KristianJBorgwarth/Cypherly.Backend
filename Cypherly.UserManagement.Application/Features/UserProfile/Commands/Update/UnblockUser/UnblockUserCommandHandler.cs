using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.UnblockUser;

public class UnblockUserCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IUserProfileService userProfileService,
    ILogger<UnblockUserCommandHandler> logger) 
    : ICommandHandler<UnblockUserCommand>
{
    public async Task<Result> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.Id);
            if (userProfile is null)
            {
                logger.LogCritical("User with {ID} not found", request.Id);
                return Result.Fail(Errors.General.NotFound(request.Id));
            }
            
            var userToUnblock = await userProfileRepository.GetByUserTag(request.Tag);
            if (userToUnblock is null)
            {
                logger.LogCritical("User with tag {Tag} not found", request.Tag);
                return Result.Fail(Errors.General.NotFound(request.Tag));
            }
            
            userProfileService.UnblockUser(userProfile, userToUnblock);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "An exception occured while user with {ID} tried to unblock user with tag {Tag}", request.Id, request.Tag);
            return Result.Fail(Errors.General.UnspecifiedError("An exception occured while attempting to unblock"));
        }
    }
}