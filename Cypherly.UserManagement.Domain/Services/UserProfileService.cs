using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Cypherly.UserManagement.Domain.ValueObjects;

namespace Cypherly.UserManagement.Domain.Services;

public interface IUserProfileService
{
    UserProfile CreateUserProfile(Guid userId, string username);
    bool IsUserBloccked(UserProfile userProfile, UserProfile checkUserProfile);
    void BlockUser(UserProfile userProfile, UserProfile blockedUserProfile);
    void UnblockUser(UserProfile userProfile, UserProfile unblockedUserProfile);
    void SoftDelete(UserProfile userProfile);
    void RevertSoftDelete(UserProfile userProfile);
}
public class UserProfileService : IUserProfileService
{
    public UserProfile CreateUserProfile(Guid userId, string username)
    {
        var tag = UserTag.Create(username);
        return new(userId, username, tag);
    }

    /// <summary>
    /// Checks whether the user is blocked by the checkUserProfile or vice versa
    /// </summary>
    /// <param name="userProfile"></param>
    /// <param name="checkUserProfile"></param>
    /// <returns></returns>
    public bool IsUserBloccked(UserProfile userProfile, UserProfile checkUserProfile)
    {
        return userProfile.BlockedUsers.Any(b => b.BlockedUserProfileId == checkUserProfile.Id)
               || checkUserProfile.BlockedUsers.Any(b => b.BlockedUserProfileId == userProfile.Id);
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

    public void UnblockUser(UserProfile userProfile, UserProfile unblockedUserProfile)
    {
        userProfile.UnblockUser(unblockedUserProfile.Id);
    }

    public void SoftDelete(UserProfile userProfile)
    {
        userProfile.SetDelete();
    }

    public void RevertSoftDelete(UserProfile userProfile)
    {
        userProfile.RevertDelete();
    }
}
