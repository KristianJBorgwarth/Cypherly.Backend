using Cypherly.Domain.Common;
using Cypherly.UserManagement.Domain.Entities;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Cypherly.UserManagement.Domain.ValueObjects;

namespace Cypherly.UserManagement.Domain.Aggregates;

public partial class UserProfile : AggregateRoot
{
    public string Username { get; private set; } = null!;
    public UserTag UserTag { get; private set; } = null!;
    public string? DisplayName { get; private set; }
    public string? ProfilePictureUrl { get; private set; }

    private readonly List<BlockedUser> _blockedUsers = [];
    public virtual IReadOnlyCollection<BlockedUser> BlockedUsers => _blockedUsers;

    private readonly List<Friendship> _friendshipsReceived = [];
    public virtual IReadOnlyCollection<Friendship> FriendshipsReceived => _friendshipsReceived;

    private readonly List<Friendship> _friendshipsInitiated = [];
    public virtual IReadOnlyCollection<Friendship> FriendshipsInitiated => _friendshipsInitiated;

    public UserProfile() : base(Guid.Empty) { } // For EF Core

    public UserProfile(Guid id, string username, UserTag userUserTag) : base(id)
    {
        Username = username;
        UserTag = userUserTag;
    }

    public void SetProfilePictureUrl(string profilePictureUrl)
    {
        ProfilePictureUrl = profilePictureUrl;
        AddDomainEvent(new UserProfilePictureUpdatedEvent(Id));
    }

    public Result SetDisplayName(string displayName)
    {
        if (displayName.Length < 3)
            return Result.Fail(Errors.General.ValueTooSmall(nameof(displayName), 3));
        if (displayName.Length > 20)
            return Result.Fail(Errors.General.ValueTooLarge(nameof(displayName), 20));
        if (!DisplayNameRegex().IsMatch(displayName))
            return Result.Fail(Errors.General.UnexpectedValue(nameof(displayName)));

        DisplayName = displayName;
        AddDomainEvent(new UserProfileDisplayNameUpdatedEvent(Id));
        return Result.Ok();
    }

    public Result AddFriendship(UserProfile userProfile)
    {
        if(Id == userProfile.Id)
            return Result.Fail(Errors.General.UnspecifiedError("Cannot add self as friend"));

        if(FriendshipsInitiated.Any(f=> f.FriendProfileId == userProfile.Id))
            return Result.Fail(Errors.General.UnspecifiedError("Friendship already exists"));

        if(FriendshipsReceived.Any(f=> f.UserProfileId == userProfile.Id))
            return Result.Fail(Errors.General.UnspecifiedError("Friendship already exists"));

        _friendshipsInitiated.Add(new(Guid.NewGuid(), Id, userProfile.Id));
        return Result.Ok();
    }

    public Result DeleteFriendship(string friendTag)
    {
        if (friendTag is null)
            throw new InvalidOperationException("FriendTag cannot be null");

        var friendshipInitiated = FriendshipsInitiated
            .FirstOrDefault(f => f.FriendProfile.UserTag.Tag == friendTag);

        if (friendshipInitiated is not null)
        {
            _friendshipsInitiated.Remove(friendshipInitiated);
            return Result.Ok();
        }

        // Check received friendships
        var friendshipReceived = FriendshipsReceived
            .FirstOrDefault(f => f.UserProfile.UserTag.Tag == friendTag);

        if (friendshipReceived is not null)
        {
            _friendshipsReceived.Remove(friendshipReceived);
            return Result.Ok();
        }

        return Result.Fail(Errors.General.UnspecifiedError("Friendship not found"));
    }

    public Result BlockUser(Guid blockedUserId)
    {
        if(blockedUserId == Guid.Empty)
            return Result.Fail(Errors.General.ValueIsRequired(nameof(blockedUserId)));

        if(blockedUserId == Id)
            return Result.Fail(Errors.General.UnspecifiedError("Cannot block self"));

        if (BlockedUsers.Any(b => b.BlockedUserId == blockedUserId))
            return Result.Fail(Errors.General.UnspecifiedError("User already blocked"));

        _blockedUsers.Add(new(Guid.NewGuid(), Id, blockedUserId));
        return Result.Ok();
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9]*$")]
    private static partial System.Text.RegularExpressions.Regex DisplayNameRegex();
}