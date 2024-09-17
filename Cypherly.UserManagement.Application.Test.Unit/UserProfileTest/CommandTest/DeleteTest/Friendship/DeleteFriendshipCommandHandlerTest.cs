using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.DeleteTest.Friendship;

public class DeleteFriendshipCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUow;
    private readonly IUserProfileService _fakeService;
    private readonly DeleteFriendshipCommandHandler _sut;
    
    public DeleteFriendshipCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeUow = A.Fake<IUnitOfWork>();
        _fakeService = A.Fake<IUserProfileService>();
        var fakeLogger = A.Fake<ILogger<DeleteFriendshipCommandHandler>>();
        _sut = new(_fakeRepo, _fakeUow,_fakeService, fakeLogger);
    }
    
    [Fact]
    public async Task Handle_Given_Valid_Command_Should_Return_Success()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("testUser"));

        var command = new DeleteFriendshipCommand()
        {
            FriendTag = "validTag",
            UserProfileId = userProfile.Id
        };
        
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.UserProfileId)).Returns(userProfile);
        A.CallTo(()=> _fakeService.DeleteFriendship(userProfile, command.FriendTag)).Returns(Result.Ok());
        A.CallTo(() => _fakeRepo.UpdateAsync(userProfile)).DoesNothing();
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).DoesNothing();
        
        // Act
        var result = await _sut.Handle(command, default);
        
        // Assert
        result.Success.Should().BeTrue();
        A.CallTo(()=> _fakeRepo.GetByIdAsync(command.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeService.DeleteFriendship(userProfile, command.FriendTag)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(userProfile)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Given_Command_With_Invalid_Id_Should_Return_Fail()
    {
        // Arrange
        var command = new DeleteFriendshipCommand()
        {
            FriendTag = "validTag",
            UserProfileId = Guid.NewGuid()
        };
        
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.UserProfileId)).Returns((UserProfile)null);
        
        // Act
        var result = await _sut.Handle(command, default);
        
        // Assert
        result.Success.Should().BeFalse();
        A.CallTo(()=> _fakeRepo.GetByIdAsync(command.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeService.DeleteFriendship(A<UserProfile>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeRepo.UpdateAsync(A<UserProfile>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Command_With_Invalid_FriendTag_Should_Return_Fail()
    {
        // Arrange
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("testUser"));

        var command = new DeleteFriendshipCommand()
        {
            FriendTag = "validTag",
            UserProfileId = userProfile.Id
        };
        
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.UserProfileId)).Returns(userProfile);
        A.CallTo(()=> _fakeService.DeleteFriendship(userProfile, command.FriendTag)).Returns(Result.Fail(Errors.General.UnspecifiedError("whoops")));
        
        // Act
        var result = await _sut.Handle(command, default);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("whoops");
        A.CallTo(()=> _fakeRepo.GetByIdAsync(command.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeService.DeleteFriendship(userProfile, command.FriendTag)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(userProfile)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).MustNotHaveHappened();
    }
    
    [Fact]
    public async Task Handle_Given_Uow_Throws_Exception_Should_Return_Fail()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("testUser"));
        
        var command = new DeleteFriendshipCommand()
        {
            FriendTag = "validTag",
            UserProfileId = userProfile.Id
        };
        
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.UserProfileId)).Returns(userProfile);
        A.CallTo(()=> _fakeService.DeleteFriendship(userProfile, command.FriendTag)).Returns(Result.Ok());
        A.CallTo(()=> _fakeUow.SaveChangesAsync(default)).Throws<Exception>();
        
        // Act
        var result = await _sut.Handle(command, default);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("An exception occured while attempting to delete a friendship.");
        A.CallTo(()=> _fakeRepo.GetByIdAsync(command.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeService.DeleteFriendship(userProfile, command.FriendTag)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(userProfile)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
    }
    
}