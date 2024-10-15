using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.BlockUser;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.BlockUser;

public class BlockUserCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUserProfileService _fakeService;
    private readonly IUnitOfWork _fakeUow;
    private readonly BlockUserCommandHandler _sut;
    
    public BlockUserCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeService = A.Fake<IUserProfileService>();
        _fakeUow = A.Fake<IUnitOfWork>();
        var fakeLogger = A.Fake<ILogger<BlockUserCommandHandler>>();
        _sut = new(_fakeRepo, _fakeService, _fakeUow, fakeLogger);
    }
    
    [Fact]
    public async Task Handle_WhenUserProfileIsNull_ReturnsNotFound()
    {
        // Arrange
        var request = new BlockUserCommand { Id = Guid.Empty, BlockedUserTag = "blockedUserTag" };
        A.CallTo(() => _fakeRepo.GetByIdAsync(request.Id)).Returns((UserProfile)null);
        
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }
    
    [Fact]
    public async Task Handle_WhenBlockedUserProfileIsNull_ReturnsNotFound()
    {
        // Arrange
        var request = new BlockUserCommand { Id = Guid.NewGuid(), BlockedUserTag = "blockedUserTag" };
        A.CallTo(() => _fakeRepo.GetByIdAsync(request.Id)).Returns(new UserProfile());
        A.CallTo(() => _fakeRepo.GetByUserTag(request.BlockedUserTag)).Returns((UserProfile)null);
        
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }
    
    [Fact]
    public async Task Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var request = new BlockUserCommand { Id = Guid.NewGuid(), BlockedUserTag = "blockedUserTag" };
        A.CallTo(() => _fakeRepo.GetByIdAsync(request.Id)).Throws<Exception>();
        
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("unspecified.error");
    }
    
    [Fact]
    public async Task Handle_WhenAllIsWell_ReturnsOk()
    {
        // Arrange
        var request = new BlockUserCommand { Id = Guid.NewGuid(), BlockedUserTag = "blockedUserTag" };
        A.CallTo(() => _fakeRepo.GetByIdAsync(request.Id)).Returns(new UserProfile());
        A.CallTo(() => _fakeRepo.GetByUserTag(request.BlockedUserTag)).Returns(new UserProfile());
        
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);
        
        // Assert
        A.CallTo(()=> _fakeService.BlockUser(A<UserProfile>._, A<UserProfile>._)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeUow.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly();
        result.Success.Should().BeTrue();
    }
}