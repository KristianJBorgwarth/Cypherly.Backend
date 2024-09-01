using AutoMapper;
using Cypherly.Authentication.Application.Features.User.Commands.Create;
using Cypherly.Authentication.Domain.Aggregates;

namespace Cypherly.Authentication.Application.Profiles;

public class UserProfiles : Profile
{
    public UserProfiles()
    {
        CreateMap<User, CreateUserDto>().ReverseMap();
    }
}