using AutoMapper;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using Cypherly.UserManagement.Domain.Services;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.QueryTest.GetUserProfileByTag;

public class GetUserProfileByTagQueryHandlerTest
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserProfileService _userProfileService;
    private readonly IProfilePictureService _profilePictureService;
    private readonly IMapper _mapper;

    private readonly GetUserProfileByTagQueryHandler _sut;

    public GetUserProfileByTagQueryHandlerTest()
    {
        _userProfileRepository = A.Fake<IUserProfileRepository>();
        _userProfileService = A.Fake<IUserProfileService>();
        _profilePictureService = A.Fake<IProfilePictureService>();
        _mapper = A.Fake<IMapper>();
        var logger = A.Fake<ILogger<GetUserProfileByTagQueryHandler>>();
        
        _sut = new(_userProfileRepository, _userProfileService, _profilePictureService, _mapper, logger);
    }
}