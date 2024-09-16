using AutoMapper;
using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.DisplayName;

public class UpdateUserProfileDisplayNameCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IMapper _fakeMapper;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly UpdateUserProfileDisplayNameCommandHandler _sut;

    public UpdateUserProfileDisplayNameCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeMapper = A.Fake<IMapper>();
        var fakeLogger = A.Fake<ILogger<UpdateUserProfileDisplayNameCommandHandler>>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _sut = new(_fakeRepo, _fakeMapper, fakeLogger, _fakeUnitOfWork);
    }

    [Fact]
    public async Task Handle_Given_ValidCommand_Should_ReturnDto_And_Result_Ok()
    {
        // Arrange
        var testProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var cmd = new UpdateUserProfileDisplayNameCommand
        {
            DisplayName = "validDisplayName",
            UserProfileId = testProfile.Id
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.UserProfileId)).Returns(testProfile);
        A.CallTo(()=> _fakeRepo.UpdateAsync(testProfile)).DoesNothing();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).DoesNothing();
        A.CallTo(()=> _fakeMapper.Map<UpdateUserProfileDisplayNameDto>(testProfile)).Returns(new()
        {
            DisplayName = cmd.DisplayName
        });

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.DisplayName.Should().Be(cmd.DisplayName);
        A.CallTo(()=> _fakeRepo.GetByIdAsync(cmd.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeRepo.UpdateAsync(testProfile)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeMapper.Map<UpdateUserProfileDisplayNameDto>(testProfile)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Given_Command_With_Invalid_UserProfileId_Should_Return_Result_Fail()
    {
        // Arrrange
        var cmd = new UpdateUserProfileDisplayNameCommand
        {
            DisplayName = "validDisplayName",
            UserProfileId = Guid.NewGuid()
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.UserProfileId)).Returns((UserProfile)null);

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeFalse();
        A.CallTo(()=> _fakeRepo.GetByIdAsync(cmd.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeRepo.UpdateAsync(A<UserProfile>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
        A.CallTo(()=> _fakeMapper.Map<UpdateUserProfileDisplayNameDto>(A<UserProfile>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Command_With_Invalid_DisplayName_Should_Return_Result_Fail()
    {
        // Arrange
        var testProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var cmd = new UpdateUserProfileDisplayNameCommand
        {
            DisplayName = "", // invalid
            UserProfileId = testProfile.Id
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.UserProfileId)).Returns(testProfile);

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeFalse();
        A.CallTo(()=> _fakeRepo.GetByIdAsync(cmd.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeRepo.UpdateAsync(A<UserProfile>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
        A.CallTo(()=> _fakeMapper.Map<UpdateUserProfileDisplayNameDto>(A<UserProfile>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Command_With_Exception_Should_Return_Result_Fail()
    {
        // Arrange
        var testProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var cmd = new UpdateUserProfileDisplayNameCommand
        {
            DisplayName = "validDisplayName",
            UserProfileId = testProfile.Id
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.UserProfileId)).Returns(testProfile);
        A.CallTo(()=> _fakeRepo.UpdateAsync(testProfile)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeFalse();
        A.CallTo(()=> _fakeRepo.GetByIdAsync(cmd.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeRepo.UpdateAsync(testProfile)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
        A.CallTo(()=> _fakeMapper.Map<UpdateUserProfileDisplayNameDto>(A<UserProfile>._)).MustNotHaveHappened();
    }

}