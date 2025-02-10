using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;

public sealed class GetBlockedUserProfilesQueryHandler(
    IUserProfileRepository repository,
    ILogger<GetBlockedUserProfilesQueryHandler> logger) 
    : IQueryHandler<GetBlockedUserProfilesQuery, List<GetBlockedUserProfilesDto>>
{
    public async Task<Result<List<GetBlockedUserProfilesDto>>> Handle(GetBlockedUserProfilesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await repository.GetByIdAsync(query.UserId);
            if (userProfile is null)
            {
                logger.LogError("UserProfile with ID: {UserId} not found", query.UserId);
                return Result.Fail<List<GetBlockedUserProfilesDto>>(Errors.General.NotFound(query.UserId));
            }

            var blockedUserProfiles = userProfile.BlockedUsers
                .Select(f => GetBlockedUserProfilesDto.MapFrom(f.BlockedUserProfile))
                .ToList();
            
            return Result.Ok(blockedUserProfiles);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occurred in GetBlockedUserProfilesQueryHandler for UserProfile with ID: {UserId}", query.UserId);
            return Result.Fail<List<GetBlockedUserProfilesDto>>(Errors.General.UnspecifiedError("An exception occured during the request"));
        }
    }
}