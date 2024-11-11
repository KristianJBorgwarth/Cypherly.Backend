﻿using AutoMapper;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Create;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using Cypherly.Common.Messaging.Messages.RequestMessages.User.Create;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.CommandTest.CreateTest;

public class CreateUserCommandHandlerTest : IntegrationTestBase
{
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {

        var scope = factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserLifeCycleServices>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var requestClient = scope.ServiceProvider.GetRequiredService<IRequestClient<CreateUserProfileRequest>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateUserCommandHandler>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();

        _sut = new CreateUserCommandHandler(userRepository, userService, deviceService, unitOfWork, requestClient, logger);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ReturnsFailure()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("hello@test.dk"), Password.Create("validPassword=?23"), false);

        await Db.User.AddAsync(user);
        await Db.SaveChangesAsync();

        var command = new CreateUserCommand()
        {
            Email = user.Email.Address,
            Password = "validPassword=?23",
            Username = "validUsername",
            DeviceAppVersion = "1.0",
            DeviceName = "deviceName",
            DevicePlatform = Domain.Enums.DevicePlatform.Android,
            DevicePublicKey = "devicePublicKey"
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("An account already exists with that email");
        Db.User.Should().HaveCount(1);
        Db.Device.Should().HaveCount(0);
        Db.OutboxMessage.Should().HaveCount(0);
    }

    [Fact]
    public async Task Handle_WhenUserCreationFails_ReturnsFailure()
    {
        // Arrange
        var command = new CreateUserCommand()
        {
            Email = "wrong email",
            Password = "wrong password",
            Username = "validUsername",
            DeviceAppVersion = "1.0",
            DeviceName = "deviceName",
            DevicePlatform = Domain.Enums.DevicePlatform.Android,
            DevicePublicKey = "devicePublicKey"
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Invalid email address.");
        Db.User.Should().HaveCount(0);
        Db.Device.Should().HaveCount(0);
        Db.OutboxMessage.Should().HaveCount(0);
    }

    [Fact]
    public async Task Handle_WhenProfileCreationFails_ReturnsFailure()
    {

    }

    [Fact]
    public async Task Handle_WhenUserCreationSucceeds_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateUserCommand()
        {
            Email = "valid@email.dk",
            Password = "validPassword=?23",
            Username = "validUsername",
            DeviceAppVersion = "1.0",
            DeviceName = "deviceName",
            DevicePlatform = Domain.Enums.DevicePlatform.Android,
            DevicePublicKey = "devicePublicKey"
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Harness.Published.Select<CreateUserProfileRequest>().Where(cr=> cr.Context.Message.Username == "validUsername").Should().HaveCount(1);
        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
        Db.User.Should().HaveCount(1);
        Db.Device.Should().HaveCount(1);
        Db.OutboxMessage.Count().Should().Be(1);
    }
}