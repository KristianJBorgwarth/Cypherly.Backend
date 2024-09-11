using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Domain.Aggregates;

namespace Cypherly.Authentication.Application.Contracts;

public interface IClaimRepository : IRepository<Claim>
{
    public Task<bool> DoesClaimExistAsync(string claimType);
}