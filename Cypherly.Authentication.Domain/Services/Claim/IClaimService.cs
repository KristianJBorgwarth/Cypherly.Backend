using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.Claim;
//TODO: TEST THIS

public interface IClaimService
{
    Result<Aggregates.Claim> CreateClaim(string claimType);
}

public class ClaimService : IClaimService
{
    
    public Result<Aggregates.Claim> CreateClaim(string claimType)
    {
        switch (claimType.Length)
        {
            case 0:
                return Result.Fail<Aggregates.Claim>(Errors.General.ValueIsEmpty(nameof(claimType)));
            case > 30:
                return Result.Fail<Aggregates.Claim>(Errors.General.ValueTooLarge(nameof(claimType), 30));
            default:
            {
                var formattedClaimType = claimType.ToLower();
        
                return Result.Ok(new Aggregates.Claim(Guid.NewGuid(), formattedClaimType));
            }
        }
    }
}