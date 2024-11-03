using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Events.UserProfile;

namespace Cypherly.UserManagement.Domain.Services;

public interface IUserBlockingService
{
    bool IsUserBloccked(UserProfile userProfile, UserProfile checkUserProfile);
    void BlockUser(UserProfile userProfile, UserProfile blockedUserProfile);
    void UnblockUser(UserProfile userProfile, UserProfile unblockedUserProfile);

}
public class UserBlockingService : IUserBlockingService
{
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

    /// <summary>
    ///  Unblock a user by removing their id from the blocked users list
    /// </summary>
    /// <param name="userProfile">The unblocking UserProfile <see cref="UserProfile"/></param>
    /// <param name="unblockedUserProfile">The user that will be unblocked <see cref="UserProfile"/></param>
    public void UnblockUser(UserProfile userProfile, UserProfile unblockedUserProfile)
    {
        userProfile.UnblockUser(unblockedUserProfile.Id);
    }
}