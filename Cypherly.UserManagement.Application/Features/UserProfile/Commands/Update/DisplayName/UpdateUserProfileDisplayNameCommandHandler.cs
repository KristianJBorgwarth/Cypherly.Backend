using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;

public class UpdateUserProfileDisplayNameCommandHandler(
    IUserProfileRepository userProfileRepository,
    IMapper mapper,
    ILogger<UpdateUserProfileDisplayNameCommandHandler> logger,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateUserProfileDisplayNameCommand, UpdateUserProfileDisplayNameDto>
{
    public async Task<Result<UpdateUserProfileDisplayNameDto>> Handle(UpdateUserProfileDisplayNameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserProfileId);
            if (userProfile is null)
            {
                logger.LogWarning("User profile with id: {Id} not found.", request.UserProfileId);
                return Result.Fail<UpdateUserProfileDisplayNameDto>(Errors.General.NotFound("User profile not found."));
            }

            var result = userProfile.SetDisplayName(request.DisplayName);
            if (result.Success is false) return Result.Fail<UpdateUserProfileDisplayNameDto>(result.Error);

            await userProfileRepository.UpdateAsync(userProfile);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = mapper.Map<UpdateUserProfileDisplayNameDto>(userProfile);
            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Command} for UserProfile with id: {Id}", nameof(UpdateUserProfileDisplayNameCommand), request.UserProfileId);
            return Result.Fail<UpdateUserProfileDisplayNameDto>(Errors.General.UnspecifiedError("An exception occurred while updating the user profile display name."));
        }
    }
}