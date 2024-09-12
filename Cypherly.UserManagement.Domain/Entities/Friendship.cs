using Cypherly.Domain.Common;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Enums;

namespace Cypherly.UserManagement.Domain.Entities;

public class Friendship : Entity
{
    public Guid UserProfileId { get; private set; }
    public Guid FriendProfileId { get; private set; }

    public FriendshipStatus Status { get; private set; }

    public virtual UserProfile UserProfile { get; private set; } = null!;
    public virtual UserProfile FriendProfile { get; private set; } = null!;

    public Friendship() : base(Guid.Empty) {} // Required for EF Core

    public Friendship(Guid id, Guid userProfileId, Guid friendProfileId) : base(id)
    {
        UserProfileId = userProfileId;
        FriendProfileId = friendProfileId;
        Status = FriendshipStatus.Pending;
    }
}