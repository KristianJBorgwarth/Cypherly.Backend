using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Entities;
using Cypherly.UserManagement.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;

public class GetFriendsQueryHandler(
    IUserProfileRepository userProfileRepository,
    ILogger<GetFriendsQueryHandler> logger)
    : IQueryHandler<GetFriendsQuery, List<GetFriendsDto>>
{
    public async Task<Result<List<GetFriendsDto>>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserProfileId);
            if (userProfile is null)
            {
                return Result.Fail<List<GetFriendsDto>>(Errors.General.NotFound(request.UserProfileId));
            }

            var friendDtos = new List<GetFriendsDto>();

            // Map initiated friendships
            friendDtos.AddRange(userProfile.FriendshipsInitiated
                .Where(f => f.Status == FriendshipStatus.Accepted)
                .Select(f => MapFriendshipToDto(f, initiatedByUser: true)));

            // Map received friendships
            friendDtos.AddRange(userProfile.FriendshipsReceived
                .Where(f => f.Status == FriendshipStatus.Accepted)
                .Select(f => MapFriendshipToDto(f, initiatedByUser: false)));

            return Result.Ok(friendDtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred in {Handler}, while attempting to retrieve friends for {UserProfileId}",
                nameof(GetFriendsQueryHandler), request.UserProfileId);
            return Result.Fail<List<GetFriendsDto>>(Errors.General.UnspecifiedError("An exception occurred while attempting to retrieve friends."));
        }
    }

    private static GetFriendsDto MapFriendshipToDto(Friendship friendship, bool initiatedByUser)
    {
        var friendProfile = initiatedByUser ? friendship.FriendProfile : friendship.UserProfile;
        return new()
        {
            Username = friendProfile.Username,
            UserTag = friendProfile.UserTag.Tag,
            DisplayName = friendProfile.DisplayName,
        };
    }
}