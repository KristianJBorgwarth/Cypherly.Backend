using AutoMapper;
using Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;
using Cypherly.Authentication.Domain.Aggregates;

namespace Cypherly.Authentication.Application.Profiles;

public class ClaimProfiles : Profile
{
    public ClaimProfiles()
    {
        CreateMap<Claim, CreateClaimDto>().ReverseMap();
    }
}