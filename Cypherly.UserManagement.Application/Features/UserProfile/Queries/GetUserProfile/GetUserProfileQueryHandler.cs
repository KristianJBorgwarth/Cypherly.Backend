using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    IMapper mapper,
    ILogger<GetUserProfileQueryHandler> logger)
    : IQueryHandler<GetUserProfileQuery, GetUserProfileDto>
{
    public async Task<Result<GetUserProfileDto>> Handle(GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userprofile = await userProfileRepository.GetByIdAsync(request.UserProfileId);
            if (userprofile is null)
                return Result.Fail<GetUserProfileDto>(Errors.General.NotFound(request.UserProfileId.ToString()));

            var profilePictureUrl = "";

            if (!string.IsNullOrEmpty(userprofile.ProfilePictureUrl))
            {
                var presignedUrlResult =
                    await profilePictureService.GetPresignedProfilePictureUrlAsync(userprofile.ProfilePictureUrl);
                if (presignedUrlResult.Success is false)
                {
                    logger.LogWarning("Failed to get presigned url for profile picture with key {Key}",
                        userprofile.ProfilePictureUrl);
                }
                else
                {
                    profilePictureUrl = presignedUrlResult.Value;
                }
            }

            var dto = mapper.Map<GetUserProfileDto>(userprofile);
            dto = dto with { ProfilePictureUrl = profilePictureUrl };

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occured while handling request with id {Id}", request.UserProfileId);
            return Result.Fail<GetUserProfileDto>(
                Errors.General.UnspecifiedError("An exception occured while handling the request"));
        }
    }
}