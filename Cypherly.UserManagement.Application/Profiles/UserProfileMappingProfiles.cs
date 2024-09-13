using AutoMapper;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileById;
using Cypherly.UserManagement.Domain.Aggregates;

namespace Cypherly.UserManagement.Application.Profiles;

public class UserProfileMappingProfiles : Profile
{
    public UserProfileMappingProfiles()
    {
        CreateMap<UserProfile, GetUserProfileByIdDto>()
            .ForMember(dest => dest.UserTag, opt => opt.MapFrom(src => src.UserTag.Tag)) // Map the Tag property of UserTag
            .ReverseMap();
    }
}