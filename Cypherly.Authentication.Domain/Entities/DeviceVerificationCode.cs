using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Entities;

public class DeviceVerificationCode : Entity
{
    public VerificationCode Code { get; private set; } = null!;
    public Guid DeviceId { get; private set; }

    public DeviceVerificationCode() : base(Guid.Empty) { } // For EF Core

    public DeviceVerificationCode(Guid id, Guid deviceId) : base(id)
    {
        DeviceId = deviceId;
        Code = VerificationCode.Create(DateTime.UtcNow.AddMinutes(30));
    }
}