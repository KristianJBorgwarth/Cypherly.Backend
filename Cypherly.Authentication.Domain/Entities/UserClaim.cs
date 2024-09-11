using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Domain.Common;
// ReSharper disable ConvertToPrimaryConstructor

namespace Cypherly.Authentication.Domain.Entities;

public class UserClaim : Entity
{
    public Guid UserId { get; private set; } 
    public Guid ClaimId { get; private set; }

    public virtual User User { get; private set; } = null!;
    public virtual Claim Claim { get; private set; } = null!;

    public UserClaim() : base(Guid.Empty) {} // For EF Core

    public UserClaim(Guid id, Guid userId, Guid claimId) : base(id)
    {
        UserId = userId;
        ClaimId = claimId;
    }
}