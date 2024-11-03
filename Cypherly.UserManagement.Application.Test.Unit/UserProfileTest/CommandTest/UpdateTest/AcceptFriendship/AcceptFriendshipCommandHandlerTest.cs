using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.AcceptFriendship;

public class AcceptFriendshipCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IFriendshipService _fakeService;
    private readonly IUnitOfWork _fakeUow;
    private readonly AcceptFriendshipCommandHandler _sut;

    public AcceptFriendshipCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeService = A.Fake<IFriendshipService>();
        _fakeUow = A.Fake<IUnitOfWork>();
        var fakeLogger = A.Fake<ILogger<AcceptFriendshipCommandHandler>>();
        _sut = new AcceptFriendshipCommandHandler(_fakeRepo, _fakeService, _fakeUow, fakeLogger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Command_Should_Create_Friendship_And_Return_ResultOk()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "friend",
            Id = userProfile.Id
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).Returns(userProfile);
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).Returns(Result.Ok());
        A.CallTo(() => _fakeRepo.UpdateAsync(userProfile)).Returns(Task.CompletedTask);
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(userProfile)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Given_Invalid_Id_Should_Return_ResultFail()
    {
        // Arrange
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "friend",
            Id = Guid.NewGuid()
        };
        A.CallTo(()=> _fakeRepo.GetByIdAsync(command.Id)).Returns((UserProfile)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(command.Id));
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.AcceptFriendship(A<UserProfile>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeRepo.UpdateAsync(A<UserProfile>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Invalid_FriendTag_Should_Return_ResultFail()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "friend",
            Id = userProfile.Id
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).Returns(userProfile);
        A.CallTo(()=> _fakeService.AcceptFriendship(userProfile, command.FriendTag)).Returns(Result.Fail(Errors.General.UnspecifiedError("Friendship not found")));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.UnspecifiedError("Friendship not found"));
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(A<UserProfile>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Exception_Should_Return_ResultFail()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "friend",
            Id = userProfile.Id
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).Returns(userProfile);
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.UnspecifiedError("An exception occured while accepting friendship"));
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(A<UserProfile>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }
}