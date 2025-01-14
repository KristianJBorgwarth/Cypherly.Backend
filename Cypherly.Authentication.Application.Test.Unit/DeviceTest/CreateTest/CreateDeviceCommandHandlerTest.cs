using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Caching.LoginNonce;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Device.Commands.Create;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Domain.Common;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.DeviceTest.CreateTest;

public class CreateDeviceCommandHandlerTest
{
    private readonly IUserRepository _fakeRepo;
    private readonly ILoginNonceCache _fakeLoginNonceCache;
    private readonly IDeviceService _fakeDeviceService;
    private readonly IUnitOfWork _fakeUnitOfWork;

    private readonly CreateDeviceCommandHandler _sut;

    public CreateDeviceCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserRepository>();
        _fakeLoginNonceCache = A.Fake<ILoginNonceCache>();
        _fakeDeviceService = A.Fake<IDeviceService>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();

        _sut = new CreateDeviceCommandHandler(_fakeRepo, _fakeLoginNonceCache, _fakeDeviceService, _fakeUnitOfWork, A.Fake<ILogger<CreateDeviceCommandHandler>>());
    }


    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new CreateDeviceCommand
        {
            UserId = Guid.NewGuid(),
            LoginNonceId = default,
            LoginNonce = null,
            DeviceAppVersion = null,
            DeviceType = (DeviceType)0,
            DevicePlatform = (DevicePlatform)0,
            Base64DevicePublicKey = null
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(request.UserId)).Returns((User)null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(request.UserId));
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._, A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeLoginNonceCache.GetNonceAsync(A<Guid>._, CancellationToken.None)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenLoginNonceNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateDeviceCommand
        {
            UserId = Guid.NewGuid(),
            LoginNonceId = Guid.NewGuid(),
            LoginNonce = "nonce",
            DeviceAppVersion = null,
            DeviceType = (DeviceType)0,
            DevicePlatform = (DevicePlatform)0,
            Base64DevicePublicKey = null
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(request.UserId)).Returns(new User());
        A.CallTo(() => _fakeLoginNonceCache.GetNonceAsync(request.LoginNonceId, CancellationToken.None)).Returns((LoginNonce)null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.Unauthorized());
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._, A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenLoginNonceInvalid_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateDeviceCommand
        {
            UserId = Guid.NewGuid(),
            LoginNonceId = Guid.NewGuid(),
            LoginNonce = "nonce",
            DeviceAppVersion = null,
            DeviceType = (DeviceType)0,
            DevicePlatform = (DevicePlatform)0,
            Base64DevicePublicKey = null
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(request.UserId)).Returns(new User());
        A.CallTo(() => _fakeLoginNonceCache.GetNonceAsync(request.LoginNonceId, CancellationToken.None)).Returns(LoginNonce.Create(Guid.NewGuid()));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.Unauthorized());
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._, A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var request = new CreateDeviceCommand
        {
            UserId = Guid.NewGuid(),
            LoginNonceId = Guid.NewGuid(),
            LoginNonce = "nonce",
            DeviceAppVersion = null,
            DeviceType = (DeviceType)0,
            DevicePlatform = (DevicePlatform)0,
            Base64DevicePublicKey = null
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(request.UserId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.UnspecifiedError("An exception occured while creating device"));
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustNotHaveHappened();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._, A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustNotHaveHappened();
    }
}