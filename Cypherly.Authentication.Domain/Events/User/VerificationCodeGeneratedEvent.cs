using Cypherly.Authentication.Domain.Enums;
using Cypherly.Domain.Events;

namespace Cypherly.Authentication.Domain.Events.User;

public sealed record VerificationCodeGeneratedEvent(Guid UserId, VerificationCodeType CodeType) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}