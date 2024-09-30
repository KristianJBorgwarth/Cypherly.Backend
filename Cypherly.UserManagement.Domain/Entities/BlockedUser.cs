using Cypherly.Domain.Common;

namespace Cypherly.UserManagement.Domain.Entities;

public class BlockedUser : Entity
{
    public Guid BlockingUserId { get; private set; }
    public Guid BlockedUserId { get; private set; }

    public BlockedUser() : base(Guid.Empty) { } // EF Core
    
    public BlockedUser(Guid id, Guid blockingUserId, Guid blockedUserId) : base(id)
    {
        BlockingUserId = blockingUserId;
        BlockedUserId = blockedUserId;
    }
}