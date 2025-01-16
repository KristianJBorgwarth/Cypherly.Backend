﻿using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Device.Queries.GetDevices;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using Cypherly.Domain.Common;
using Cypherly.Domain.ValueObjects;
using FluentAssertions;
using MassTransit.SqlTransport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.DeviceTest.QueryTest.GetDevicesTest;

public class GetDevicesQueryHandlerTest : IntegrationTestBase
{
    private readonly GetDevicesQueryHandler _sut;
    public GetDevicesQueryHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var repo = serviceProvider.GetRequiredService<IUserRepository>();
        var logger = serviceProvider.GetRequiredService<ILogger<GetDevicesQueryHandler>>();

        _sut = new GetDevicesQueryHandler(repo, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Query_Should_Return_Devices()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("testPw=??kkl89"), true);

        var device1 = new Device(Guid.NewGuid(), "someKey", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        var device2 = new Device(Guid.NewGuid(), "someKey", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        user.AddDevice(device1);
        user.AddDevice(device2);

        await Db.AddAsync(user);
        await Db.SaveChangesAsync();

        var query = new GetDevicesQuery()
        {
            UserId = user.Id
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Devices.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Given_Invalid_Query_Should_Return_Error()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("testPw=??kkl89"), true);

        var device1 = new Device(Guid.NewGuid(), "someKey", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        var device2 = new Device(Guid.NewGuid(), "someKey", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        user.AddDevice(device1);
        user.AddDevice(device2);

        await Db.AddAsync(user);
        await Db.SaveChangesAsync();

        var query = new GetDevicesQuery()
        {
            UserId = Guid.NewGuid() // will be invalid
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(query.UserId));
    }

    [Fact]
    public async Task Handle_Valid_Query_With_No_Devices_Should_Return_Empty_List()
    {
        // Arrange 
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test89?klsdKK"), true);
        
        var device = new Device(Guid.NewGuid(), "somekey", "1.0", DeviceType.Desktop, DevicePlatform.Windows, user.Id);
        device.SetDelete();
        
        user.AddDevice(device);
        await Db.AddAsync(user);
        await Db.SaveChangesAsync();

        var query = new GetDevicesQuery()
        {
            UserId = user.Id
        };
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        
        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Devices.Count.Should().Be(0);
    }
}