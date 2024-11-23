using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;

public class VerifyNonceCommandHandler(
    IUserRepository userRepository,
    INonceCacheService nonceCacheService,
    ILogger<VerifyNonceCommandHandler> logger)
    : ICommandHandler<VerifyNonceCommand, VerifyNonceDto>
{
    public Task<Result<VerifyNonceDto>> Handle(VerifyNonceCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}