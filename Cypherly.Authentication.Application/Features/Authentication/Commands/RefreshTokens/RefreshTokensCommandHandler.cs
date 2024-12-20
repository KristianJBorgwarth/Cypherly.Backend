using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.RefreshTokens;

public class RefreshTokensCommandHandler : ICommandHandler<RefreshTokensCommand, RefreshTokensDto>
{
    public async Task<Result<RefreshTokensDto>> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}