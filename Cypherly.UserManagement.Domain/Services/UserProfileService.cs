using Cypherly.Domain.Common;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Enums;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Cypherly.UserManagement.Domain.ValueObjects;

namespace Cypherly.UserManagement.Domain.Services;

public interface IUserProfileService
{
    UserProfile CreateUserProfile(Guid userId, string username);
    Result CreateFriendship(UserProfile userProfile, UserProfile friendProfile);
    Result AcceptFriendship(UserProfile userProfile, string friendTag);

    Result DeleteFriendship(UserProfile userProfile, string friendTag);
    void BlockUser(UserProfile userProfile, UserProfile blockedUserProfile);
}
public class UserProfileService : IUserProfileService
{
    public UserProfile CreateUserProfile(Guid userId, string username)
    {
        var tag = UserTag.Create(username);
        return new(userId, username, tag);
    }

    public Result CreateFriendship(UserProfile userProfile, UserProfile friendProfile)
    {
        var result = userProfile.AddFriendship(friendProfile);

        if (result.Success is false)
            return Result.Fail(result.Error);

        userProfile.AddDomainEvent(new FriendshipCreatedEvent(userProfile.Id, friendProfile.Id));
        return Result.Ok();
    }

    public Result AcceptFriendship(UserProfile userProfile, string friendTag)
    {
        var friendship = userProfile.FriendshipsReceived.FirstOrDefault(f => f.UserProfile.UserTag.Tag == friendTag);

        if(friendship is null)
            return Result.Fail(Errors.General.UnspecifiedError("Friendship not found"));

        if(friendship.Status != FriendshipStatus.Pending)
            return Result.Fail(Errors.General.UnspecifiedError("Friendship not pending"));

        friendship.AcceptFriendship();
        userProfile.AddDomainEvent(new FriendshipAcceptedEvent(friendship.UserProfile.Id, userProfile.UserTag.Tag));
        return Result.Ok();
    }

    public Result DeleteFriendship(UserProfile userProfile, string friendTag)
    {
        return userProfile.DeleteFriendship(friendTag);
    }

    /// <summary>
    /// Block a user by adding their id to the blocked users list and removing the friendship
    /// </summary>
    /// <param name="userProfile">The blocking UserProfile <see cref="UserProfile"/></param>
    /// <param name="blockedUserProfile">The user that will be blocked <see cref="UserProfile"/></param>
    public void BlockUser(UserProfile userProfile, UserProfile blockedUserProfile)
    {
        userProfile.BlockUser(blockedUserProfile.Id);
        userProfile.DeleteFriendship(blockedUserProfile.UserTag.Tag);
        blockedUserProfile.DeleteFriendship(userProfile.UserTag.Tag);
        userProfile.AddDomainEvent(new UserBlockedEvent(userProfile.Id, blockedUserProfile.Id));
    }
}
