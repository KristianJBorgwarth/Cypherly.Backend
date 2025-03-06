using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfilePicture;

public sealed class GetUserProfilePictureQueryHandler(
    IMinioProxyClient minioProxyClient,
    ILogger<GetUserProfilePictureQueryValidator> logger)
    : IQueryHandler<GetUserProfilePictureQuery, GetUserProfilePictureDto>
{
    public async Task<Result<GetUserProfilePictureDto>> Handle(GetUserProfilePictureQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await minioProxyClient.GetImageFromMinioAsync(request.ProfilePictureUrl, cancellationToken);

            if (data.HasValue is false)
            {
                return Result.Fail<GetUserProfilePictureDto>(Errors.General.UnspecifiedError("Error getting user profile picture"));
            }

            var dto = GetUserProfilePictureDto.MapFrom(data.Value.image, data.Value.imageType);
            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting user profile picture");
            return Result.Fail<GetUserProfilePictureDto>(Errors.General.UnspecifiedError("Exception occured while getting user profile picture"));
        }
    }
}