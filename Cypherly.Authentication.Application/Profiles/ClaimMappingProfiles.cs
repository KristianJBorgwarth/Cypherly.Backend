using AutoMapper;
using Cypherly.Authentication.Application.Features.Claim.Commands.Create;
using Cypherly.Authentication.Domain.Aggregates;

namespace Cypherly.Authentication.Application.Profiles;

public class ClaimMappingProfiles : Profile
{
    public ClaimMappingProfiles()
    {
        CreateMap<Claim, CreateClaimDto>().ReverseMap();
    }
}