using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.ProfilePicture;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.UserManagement.Application.Test.Unit.UserProfileTest.CommandTest.UpdateTest.ProfilePicture;

public class UpdateUserProfilePictuerCommandHandler
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IProfilePictureService _fakeProfilePicService;
    private readonly IUnitOfWork _fakeUow;
    private readonly UpdateUserProfilePictureCommandHandler _sut;

    public UpdateUserProfilePictuerCommandHandler()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeProfilePicService = A.Fake<IProfilePictureService>();
        _fakeUow = A.Fake<IUnitOfWork>();
        var logger = A.Fake<ILogger<UpdateUserProfilePictureCommandHandler>>();
        _sut = new(_fakeRepo, _fakeProfilePicService, _fakeUow, logger);
    }

    [Fact]
    public async void Handle_GivenValidCommand_ShouldUpdateProfilePicture_AndReturn_UpdatedUser()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand
        {
            Id = Guid.NewGuid(),
            NewProfilePicture = A.Fake<IFormFile>()
        };

        var userProfile = new UserProfile(Guid.NewGuid(), "test", UserTag.Create("test"));

        A.CallTo(() => _fakeRepo.GetByIdAsync(command.Id)).Returns(userProfile);

        A.CallTo(() => _fakeProfilePicService.UploadProfilePictureAsync(command.NewProfilePicture, command.Id))
            .Returns(Result.Ok("somestring"));

        A.CallTo(() => _fakeUow.SaveChangesAsync(A<System.Threading.CancellationToken>._)).DoesNothing();

        // Act
        var result = await _sut.Handle(command, new());

        // Assert
        result.Success.Should().BeTrue();
        result.Value.ProfilePictureUrl.Should().Be("somestring");
    }

    [Fact]
    public async void Handle_Given_Invalid_Id_Should_Not_Update_ProfilePicture_And_Return_Error()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand()
        {
            Id = Guid.NewGuid(),
            NewProfilePicture = A.Fake<IFormFile>()
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(command.Id))!.Returns<UserProfile>(null!);

        // Act
        var result = await _sut.Handle(command, new());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async void Handle_Given_Invalid_ProfilePicture_Should_Not_Update_ProfilePicture_And_Return_Error()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand()
        {
            Id = Guid.NewGuid(),
            NewProfilePicture = A.Fake<IFormFile>()
        };

        var userProfile = new UserProfile(Guid.NewGuid(), "test", UserTag.Create("test"));

        A.CallTo(() => _fakeRepo.GetByIdAsync(command.Id)).Returns(userProfile);
        A.CallTo(()=> _fakeProfilePicService.UploadProfilePictureAsync(command.NewProfilePicture, command.Id))
            .Returns(Result.Fail<string>(Errors.General.UnspecifiedError("Invalid profile picture")));

        // Act
        var result = await _sut.Handle(command, new());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("unspecified.error");
        result.Error.Message.Should().Be("Invalid profile picture");
    }

    [Fact]
    public async void Handle_When_Exception_Occurs_Should_Return_Error()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand()
        {
            Id = Guid.NewGuid(),
            NewProfilePicture = A.Fake<IFormFile>()
        };

        var userProfile = new UserProfile(Guid.NewGuid(), "test", UserTag.Create("test"));

        A.CallTo(() => _fakeRepo.GetByIdAsync(command.Id)).Returns(userProfile);

        A.CallTo(() => _fakeProfilePicService.UploadProfilePictureAsync(command.NewProfilePicture, command.Id))
            .Throws<Exception>();

        // Act
        var result = await _sut.Handle(command, new());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("unspecified.error");
        result.Error.Message.Should().Be("An exception was thrown during the update process");
    }
}