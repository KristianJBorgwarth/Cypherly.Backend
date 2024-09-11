using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Services.Claim;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;

public class CreateClaimCommandHandler(
    IClaimRepository claimRepository,
    IClaimService claimService,
    IUnitOfWork unitOfWork,
    ILogger<CreateClaimCommandHandler> logger,
    IMapper mapper)
    : ICommandHandler<CreateClaimCommand, CreateClaimDto>
{
    public async Task<Result<CreateClaimDto>> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if(await claimRepository.DoesClaimExistAsync(request.ClaimType))
                return Result.Fail<CreateClaimDto>(Errors.General.UnspecifiedError("Claim already exists"));

            var claimResult = claimService.CreateClaim(request.ClaimType);
            if (claimResult.Success is false)
                return Result.Fail<CreateClaimDto>(claimResult.Error);

            await claimRepository.CreateAsync(claimResult.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var claimDto = mapper.Map<CreateClaimDto>(claimResult.Value);

            return Result.Ok(claimDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating claim");
            return Result.Fail<CreateClaimDto>(Errors.General.UnspecifiedError("An exception occurred while creating the claim"));
        }
    }
}