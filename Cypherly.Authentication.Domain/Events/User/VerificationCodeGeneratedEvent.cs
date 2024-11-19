using Cypherly.Authentication.Domain.Enums;
using Cypherly.Domain.Events;

namespace Cypherly.Authentication.Domain.Events.User;

public sealed record VerificationCodeGeneratedEvent(Guid UserId, UserVerificationCodeType CodeType) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}