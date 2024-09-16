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
}
