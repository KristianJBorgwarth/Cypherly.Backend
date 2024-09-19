using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Entities;

public class RefreshToken : Entity
{
    public string Token { get; private set; } = null!;
    public DateTime Expires { get; }
    public DateTime? Revoked { get; private set; }
    public bool IsRevoked => Revoked.HasValue;
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public Guid UserId { get; private set; }

    public virtual User User { get; private set; } = null!;

    public RefreshToken() : base(Guid.Empty) {} // For EF Core

    public RefreshToken(Guid id, string token, DateTime expiryDate, Guid userId) : base(id)
    {
        Token = token;
        Expires = expiryDate;
        UserId = userId;
    }

    public void Revoke()
    {
        if(IsRevoked)
        {
            throw new InvalidOperationException("Refresh token is already revoked.");
        }

        Revoked = DateTime.UtcNow;
    }

    public bool IsValid() => !IsRevoked && !IsExpired;
}