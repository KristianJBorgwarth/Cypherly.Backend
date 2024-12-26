using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Queries.GetUserDevices;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.QueryTest.GetUserDevicesTest;

public class GetUserDevicesQueryHandlerTest : IntegrationTestBase
{
    private readonly GetUserDevicesQueryHandler _sut;

    public GetUserDevicesQueryHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory)
        : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetUserDevicesQueryHandler>>();

        _sut = new(userRepository, logger);
    }

    [Fact]
    public async void Handle_Given_Valid_Query_Should_Return_Devices()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"),
            Password.Create("kjsidlæ??238Ja"), true);

        var device1 = new Device(Guid.NewGuid(), "SomeKey", "1.0", DeviceType.Mobile,
            DevicePlatform.Android, user.Id);
        var device2 = new Device(Guid.NewGuid(), "SomeKey2", "1.0", DeviceType.Desktop,
            DevicePlatform.Android, user.Id);

        var device3 = new Device(Guid.NewGuid(), "SomeKey2", "1.0", DeviceType.Desktop,
            DevicePlatform.Android, user.Id);

        device1.AddDeviceVerificationCode();
        var code = device1.GetActiveVerificationCode();
        device1.Verify(code.Code.Value);

        device2.AddDeviceVerificationCode();
        var code2 = device2.GetActiveVerificationCode();
        device2.Verify(code2.Code.Value);

        user.AddDevice(device1);
        user.AddDevice(device2);

        Db.Add(user);
        await Db.SaveChangesAsync();

        var query = new GetUserDevicesQuery { UserId = user.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Devices.Should().HaveCount(2);
    }

    [Fact]
    public async void Handle_Given_Invalid_Query_Should_Return_NotFound()
    {
        // Arrange
        var query = new GetUserDevicesQuery { UserId = Guid.NewGuid() };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async void Handle_Given_User_With_No_Devices_Should_Return_Empty_List()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"),
            Password.Create("kjsidlæ??238Ja"), true);

        Db.Add(user);
        await Db.SaveChangesAsync();

        var query = new GetUserDevicesQuery { UserId = user.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Devices.Should().BeEmpty();
    }
}
