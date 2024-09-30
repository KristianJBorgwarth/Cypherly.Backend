using Cypherly.Domain.Common;

namespace Cypherly.UserManagement.Domain.Entities;

public class BlockedUser : Entity
{
    public Guid BlockingUserId  { get; private set; }
    public Guid BlockedUserId  { get; private set; }

    public BlockedUser() : base(Guid.Empty) {} // Required for EF Core

    public BlockedUser(Guid id, Guid blockingUser, Guid blockedUser) : base(id)
    {
        BlockingUserId = blockingUser;
        BlockedUserId = blockedUser;
    }
}