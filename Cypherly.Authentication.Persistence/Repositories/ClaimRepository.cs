using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cypherly.Authentication.Persistence.Repositories;

public class ClaimRepository(AuthenticationDbContext context) : IClaimRepository
{
    public async Task CreateAsync(Claim entity)
    {
        await context.Claim.AddAsync(entity);
    }

    public Task DeleteAsync(Claim entity)
    {
        throw new NotImplementedException();
    }
    public Task<Claim?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Claim entity)
    {
        context.Claim.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> DoesClaimExistAsync(string claimType)
    {
        return await context.Claim.AnyAsync(c => c.ClaimType.Equals(claimType));
    }

    public Task<Claim?> GetClaimByTypeAsync(string claimType, CancellationToken cancellationToken)
    {
        return context.Claim.FirstOrDefaultAsync(c => c.ClaimType.Equals(claimType), cancellationToken);
    }
}