using System.Security.Cryptography;

namespace Cypherly.Authentication.Application.Caching;

public class Nonce
{
    public Guid Id { get; private init; }
    public string NonceValue { get; private init; } = null!;
    public Guid UserId { get; private init; }
    public Guid DeviceId { get; private init; }
    public DateTime CreatedAt { get; private init; }

    private DateTime _expiresAt;

    public bool Exipred => DateTime.UtcNow > _expiresAt;

    public Nonce(Guid userId, Guid deviceId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        DeviceId = deviceId;
        NonceValue = GenerateNonceValue();
        CreatedAt = DateTime.UtcNow;
        _expiresAt = DateTime.UtcNow.AddMinutes(15);
    }

    private static string GenerateNonceValue()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}