using AutoMapper;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using Cypherly.UserManagement.Domain.Aggregates;

namespace Cypherly.UserManagement.Application.Profiles;

public class UserProfileMappingProfiles : Profile
{
    public UserProfileMappingProfiles()
    {
        CreateMap<UserProfile, GetUserProfileDto>()
            .ForMember(dest => dest.UserTag, opt => opt.MapFrom(src => src.UserTag.Tag))
            .ForMember(dest=> dest.ProfilePictureUrl, opt => opt.Ignore()).ReverseMap();
        CreateMap<UserProfile, UpdateUserProfileDisplayNameDto>().ReverseMap();
        CreateMap<UserProfile, GetUserProfileByTagDto>().ReverseMap();
    }
}