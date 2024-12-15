using Cypherly.Application.Abstractions;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;

public class VerifyNonceCommand : ICommand<VerifyNonceDto>
{
    public Guid UserId { get; set; }
    public Guid NonceId { get; set; }
    public Guid DeviceId { get; set; }
    public required string Nonce { get; set; }
}