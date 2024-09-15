using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.ProfilePicture;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Application.Test.Integration.Setup.Helpers;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.CommandTest.UpdateTest.ProfilePicture;

public class UpdateUserProfilePictureCommandHandlerTest : IntegrationTestBase
{
    private readonly UpdateUserProfilePictureCommandHandler _sut;
    public UpdateUserProfilePictureCommandHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UpdateUserProfilePictureCommandHandler>>();

        _sut = new(repo, profilePictureService, unitOfWork, logger);
    }

[Fact]
    public async Task Handle_ValidUpdateCommand_Should_Update_ProfilePic_And_Return_Dto()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfilePictureCommand()
        {
            Id = user.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Cypherly.UserManagement.Application.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png")
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var updatedChatUser = await Db.UserProfile.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
        updatedChatUser!.ProfilePictureUrl.Should().NotBeNull().And.Be(result.Value.ProfilePictureUrl);
    }

    [Fact]
    public async Task Handle_Invalid_Command_WrongID_Should_Return_NotFound_Fail_Result()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfilePictureCommand()
        {
            Id = Guid.NewGuid(),
            NewProfilePicture = null
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.NotFound(command.Id).Message);
    }

    [Fact]
    public async Task Handle_Invalid_Command_WrongProfilePicture_Should_Return_Fail()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(userProfile);
        await Db.SaveChangesAsync();
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfilePictureCommand()
        {
            Id = userProfile.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Cypherly.UserManagement.Application.Test.Integration/Setup/Resources/confirm_style_2_002.wav"), "image/jpg")
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.UnspecifiedError("Value 'Invalid file type. Only JPG, JPEG and PNG files are allowed.' is not valid in this context").Message);
    }
}