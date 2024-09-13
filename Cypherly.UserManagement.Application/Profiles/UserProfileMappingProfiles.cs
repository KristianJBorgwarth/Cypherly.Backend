using AutoMapper;
using Cypherly.UserManagement.Application.Dtos;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;
using Cypherly.UserManagement.Domain.Aggregates;

namespace Cypherly.UserManagement.Application.Profiles;

public class UserProfileMappingProfiles : Profile
{
    public UserProfileMappingProfiles()
    {
        CreateMap<UserProfile, GetUserProfileDto>()
            .ForMember(dest => dest.UserTag, opt => opt.MapFrom(src => src.UserTag.Tag)).ReverseMap();

        CreateMap<UserProfile, FriendDto>().ForMember(dest=> dest.UserTag, opt=> opt.MapFrom(src=> src.UserTag.Tag)).ReverseMap();
    }
}