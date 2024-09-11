using Cypherly.Authentication.Domain.Entities;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Aggregates;

public class Claim : AggregateRoot
{
    public string ClaimType { get; private set; } = null!;
    private readonly List<UserClaim> _userClaims = [];
    public virtual IReadOnlyCollection<UserClaim> UserClaims => _userClaims.AsReadOnly();
    public Claim(): base(Guid.Empty) {} // For EF Core

    public Claim(Guid id, string claimType) : base(id)
    {
        ClaimType = claimType;
    }

    public void AddUserClaim(UserClaim userClaim) => _userClaims.Add(userClaim);
    public void RemoveUserClaim(UserClaim userClaim) => _userClaims.Remove(userClaim);
}