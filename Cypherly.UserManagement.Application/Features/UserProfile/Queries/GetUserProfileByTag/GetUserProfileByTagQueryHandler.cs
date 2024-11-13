using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public class GetUserProfileByTagQueryHandler(
    IUserProfileRepository userProfileRepository,
    IUserBlockingService userBlockingService,
    IProfilePictureService profilePictureService,
    IMapper mapper,
    ILogger<GetUserProfileByTagQueryHandler> logger)
    : IQueryHandler<GetUserProfileByTagQuery, GetUserProfileByTagDto>
{
    public async Task<Result<GetUserProfileByTagDto>> Handle(GetUserProfileByTagQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var requestingUser = await userProfileRepository.GetByIdAsync(request.Id);
            if (requestingUser is null)
            {
                logger.LogWarning("User with ID: {ID} attempted to get profile by tag: {Tag}, but no user with that ID Exists", request.Id, request.Tag);
                return Result.Fail<GetUserProfileByTagDto>(Errors.General.NotFound(request.Id));
            }

            var userProfile = await userProfileRepository.GetByUserTag(request.Tag);

            if (userProfile is null || userBlockingService.IsUserBloccked(requestingUser, userProfile) || userProfile.IsPrivate)
                return Result.Ok<GetUserProfileByTagDto>();

            var profilePictureUrl = "";

            if (!string.IsNullOrEmpty(userProfile.ProfilePictureUrl))
            {
                var presignedUrlResult = await profilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl);
                if (presignedUrlResult.Success is false)
                {
                    logger.LogWarning("Failed to get presigned url for profile picture with key {Key}", userProfile.ProfilePictureUrl);
                }
                else
                {
                    profilePictureUrl = presignedUrlResult.Value;
                }
            }

            var dto = mapper.Map<GetUserProfileByTagDto>(userProfile);
            dto = dto with { ProfilePictureUrl = profilePictureUrl };

            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Exception occured while user with ID: {ID} tried to get profile by tag: {Tag}", request.Id, request.Tag);
            return Result.Fail<GetUserProfileByTagDto>(Errors.General.UnspecifiedError("An exception occured while attempting to get the user profile by tag."));
        }
    }
}