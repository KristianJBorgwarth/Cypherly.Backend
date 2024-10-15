using AutoMapper;
using Cypherly.Application.Contracts.Repository;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;
using Cypherly.UserManagement.Application.Test.Integration.Setup;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Test.Integration.UserProfileTest.CommandTest.UpdateTest.DisplayName;

public class UpdateUserProfileDisplayNameCommandHandlerTest : IntegrationTestBase
{
    private readonly UpdateUserProfileDisplayNameCommandHandler _sut;
    public UpdateUserProfileDisplayNameCommandHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UpdateUserProfileDisplayNameCommandHandler>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        _sut = new(repo, mapper, logger, unitOfWork);
    }

    [Fact]
    public async Task Handle_Given_Valid_Command_Should_Update_UserProfile_DisplayName()
    {
        // Arrange
        var userprofile = new UserProfile(Guid.NewGuid(), "david", UserTag.Create("david"));
        Db.UserProfile.Add(userprofile);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfileDisplayNameCommand()
        {
            DisplayName = "test",
            Id = userprofile.Id
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.DisplayName.Should().Be("test");
        Db.UserProfile.AsNoTracking().FirstOrDefault(u=> u.Id == userprofile.Id)!.DisplayName.Should().Be("test");
        Db.OutboxMessage.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Given_Invalid_Command_Should_Return_Failure()
    {
        // Arrange
        var command = new UpdateUserProfileDisplayNameCommand()
        {
            DisplayName = "test",
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_Given_Invalid_Command_Should_Not_Update_UserProfile_DisplayName()
    {
        // Arrange
        var userprofile = new UserProfile(Guid.NewGuid(), "david", UserTag.Create("david"));
        Db.UserProfile.Add(userprofile);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfileDisplayNameCommand()
        {
            DisplayName = "",
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        Db.UserProfile.AsNoTracking().FirstOrDefault(u=> u.Id == userprofile.Id)!.DisplayName.Should().Be(null);
    }
}