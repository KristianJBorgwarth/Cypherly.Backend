using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.ProfilePicture;

public class UpdateUserProfilePictureCommandHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserProfilePictureCommandHandler> logger)
    : ICommandHandler<UpdateUserProfilePictureCommand, UpdateUserProfilePictureDto>
{
    public async Task<Result<UpdateUserProfilePictureDto>> Handle(UpdateUserProfilePictureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userProfileRepository.GetByIdAsync(request.Id);
            if (user is null) return Result.Fail<UpdateUserProfilePictureDto>(Errors.General.NotFound(request.Id));

            var result = await profilePictureService.UploadProfilePictureAsync(request.NewProfilePicture, request.Id);
            if (result.Success is false) return Result.Fail<UpdateUserProfilePictureDto>(result.Error);

            user.SetProfilePictureUrl(result.Value);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new UpdateUserProfilePictureDto(result.Value);

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An exception occurred in {Handler}, while attempting to update the profile picture for {UserProfileId}",
                nameof(UpdateUserProfilePictureCommandHandler), request.Id);
            return Result.Fail<UpdateUserProfilePictureDto>(
                Errors.General.UnspecifiedError("An exception was thrown during the update process"));
        }
    }
}