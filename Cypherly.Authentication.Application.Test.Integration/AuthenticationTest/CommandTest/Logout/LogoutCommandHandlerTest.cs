﻿using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Authentication.Commands.Logout;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using Cypherly.Domain.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.AuthenticationTest.CommandTest.Logout;

public class LogoutCommandHandlerTest : IntegrationTestBase
{
    private readonly LogoutCommandHandler _sut;
    public LogoutCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LogoutCommandHandler>>();

        _sut = new(repo, authService, unitOfWork, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Command_Should_Logout_User_Delete_Device_And_Related_Tokens()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("Test98??KLadds"), true);

        var device = new Device(Guid.NewGuid(), "testKey", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        device.AddRefreshToken();
        device.AddRefreshToken();
        device.AddRefreshToken();

        user.AddDevice(device);

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var command = new LogoutCommand
        {
            Id = user.Id,
            DeviceId = device.Id,
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.Device.AsNoTracking().FirstOrDefault()!.DeletedAt.Should().NotBeNull();
        Db.RefreshToken.AsNoTracking().FirstOrDefault()!.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Given_Invalid_UserId_Should_Return_Result_Fail()
    {
        // Arrange
        var command = new LogoutCommand
        {
            Id = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(command.Id));
    }

    [Fact]
    public async Task Handle_Given_No_Device_Should_Throw_Exception_And_Return_Result_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("Test98??KLadds"), true);

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var command = new LogoutCommand
        {
            Id = user.Id,
            DeviceId = Guid.NewGuid(),
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An error occured while logging out user");
    }
}