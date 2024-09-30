using Cypherly.Domain.Common;

namespace Cypherly.UserManagement.Domain.Entities;

public class BlockedUser : Entity
{
    public Guid BlockingUserProfileId { get; private set; }
    public Guid BlockedUserProfileId { get; private set; }

    public BlockedUser() : base(Guid.Empty) { } // EF Core
    
    public BlockedUser(Guid id, Guid blockingUserProfileId, Guid blockedUserProfileId) : base(id)
    {
        BlockingUserProfileId = blockingUserProfileId;
        BlockedUserProfileId = blockedUserProfileId;
    }
}