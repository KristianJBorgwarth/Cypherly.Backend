using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Entities;
using Cypherly.UserManagement.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriendRequests;

public class GetFriendRequestsQueryHandler(
    IUserProfileRepository userProfileRepository,
    ILogger<GetFriendRequestsQueryHandler> logger)
    : IQueryHandler<GetFriendRequestsQuery, List<GetFriendRequestsDto>>
{
    public async Task<Result<List<GetFriendRequestsDto>>> Handle(GetFriendRequestsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(query.UserId);
            if (userProfile is null)
            {
                logger.LogCritical("UserProfile not found, UserProfileId: {UserProfileId}", query.UserId);
                return Result.Fail<List<GetFriendRequestsDto>>(Errors.General.NotFound(query.UserId));
            }

            var friendRequests = userProfile.FriendshipsReceived
                .Where(f => f.Status == FriendshipStatus.Pending)
                .Select(f => GetFriendRequestsDto.MapFrom(f.UserProfile.Username, f.FriendProfile.UserTag.Tag, f.FriendProfile.DisplayName))
                .ToList();


            return Result.Ok(friendRequests);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetFriendRequestsQueryHandler, UserProfileId: {UserProfileId}", query.UserId);
            return Result.Fail<List<GetFriendRequestsDto>>(Errors.General.UnspecifiedError("Exception occured in GetFriendRequestsQueryHandler"));
        }
    }
}