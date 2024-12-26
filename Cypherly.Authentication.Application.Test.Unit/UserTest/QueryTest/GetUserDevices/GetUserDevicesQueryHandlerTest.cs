using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Queries.GetUserDevices;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.QueryTest.GetUserDevices;

public class GetFriendsQueryHandlerTest
{
    private readonly IUserRepository _fakeRepo;
    private readonly GetUserDevicesQueryHandler _sut;

    public GetFriendsQueryHandlerTest()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        var fakeLogger = A.Fake<ILogger<GetUserDevicesQueryHandler>>();
        _sut = new GetUserDevicesQueryHandler(_fakeRepo, fakeLogger);
    }

    [Fact]
    public async void Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetUserDevicesQuery { UserId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).Returns((User)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");

        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var query = new GetUserDevicesQuery { UserId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occurred while attempting to retrieve devices.");
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenUserExists_ReturnsDeviceList()
    {
        // Arrange
        var query = new GetUserDevicesQuery { UserId = Guid.NewGuid() };
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"),
            Password.Create("kjsidlæ??238Ja"), true);

        var device1 = new Device(Guid.NewGuid(), "SomeKey", "1.0", DeviceType.Desktop,
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


        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).Returns(user);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Devices.Should().HaveCount(2);
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).MustHaveHappenedOnceExactly();
    }
}