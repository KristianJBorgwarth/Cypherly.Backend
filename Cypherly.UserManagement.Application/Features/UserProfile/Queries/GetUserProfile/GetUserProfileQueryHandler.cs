using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IUserProfileRepository userProfileRepository,
    IMapper mapper,
    ILogger<GetUserProfileQueryHandler> logger)
    : IQueryHandler<GetUserProfileQuery, GetUserProfileDto>
{
    public async Task<Result<GetUserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userprofile = await userProfileRepository.GetByIdAsync(request.UserProfileId);
            if (userprofile is null)
                return Result.Fail<GetUserProfileDto>(Errors.General.NotFound(request.UserProfileId.ToString()));

            var dto = mapper.Map<GetUserProfileDto>(userprofile);
            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occured while handling request with id {Id}", request.UserProfileId);
            return Result.Fail<GetUserProfileDto>(Errors.General.UnspecifiedError("An exception occured while handling the request"));
        }
    }
}