using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;

public class CreateClaimCommandHandler : ICommandHandler<CreateClaimCommand>
{
    public Task<Result> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}