using Cypherly.Domain.Common;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Enums;
using Cypherly.UserManagement.Domain.Events.UserProfile;

namespace Cypherly.UserManagement.Domain.Services;

public interface IFriendshipService
{
    Result CreateFriendship(UserProfile userProfile, UserProfile friendProfile);
    Result AcceptFriendship(UserProfile userProfile, string friendTag);
    Result DeleteFriendship(UserProfile userProfile, string friendTag);
}
public class FriendshipService : IFriendshipService
{
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
}