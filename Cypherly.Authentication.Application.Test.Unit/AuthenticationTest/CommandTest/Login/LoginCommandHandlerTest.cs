using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Authentication.Commands.Login;
using Cypherly.Authentication.Application.Services.Authentication;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.AuthenticationTest.Login;

public class LoginCommandHandlerTest
{
    private readonly IUserRepository _fakeUserRepository;
    private readonly IDeviceService _fakeDeviceService;
    private readonly IUnitOfWork _fakeUnitOfWork;

    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTest()
    {
        _fakeUserRepository = A.Fake<IUserRepository>();
        _fakeDeviceService = A.Fake<IDeviceService>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();

        _sut = new LoginCommandHandler(_fakeUserRepository, _fakeDeviceService, _fakeUnitOfWork, A.Fake<ILogger<LoginCommandHandler>>());
    }

    [Fact]
    public async void Handle_Given_Invalid_Email_Should_Return_InvalidCredentials()
    {
        // Arrange
        A.CallTo(() => _fakeUserRepository.GetByEmailAsync(A<string>._)).Returns((User)null);

        var cmd = new LoginCommand()
        {
            Email = "Test@mail.dk",
            Password = "TestPassword?123",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows,
        };

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Invalid Credentials");
        A.CallTo(()=> _fakeUserRepository.GetByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._, A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_Given_Invalid_Password_Should_Return_InvalidCredentials()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("Test@mail.dk"), Password.Create("kj9203KKJHSD?23"), true);

        A.CallTo(() => _fakeUserRepository.GetByEmailAsync(A<string>._)).Returns(user);

        var cmd = new LoginCommand()
        {
            Email = "Test@mail.dk",
            Password = "THIS PASSWORD WILL BE INVALID GG",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Invalid Credentials");
        A.CallTo(()=> _fakeUserRepository.GetByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._, A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_Given_Unverified_User_Should_Return_LoginDto_With_IsVerified_False()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("Test@mail.dk"), Password.Create("kj9203KKJHSD?23"), false);

        A.CallTo(() => _fakeUserRepository.GetByEmailAsync(A<string>._)).Returns(user);

        var cmd = new LoginCommand()
        {
            Email = "Test@mail.dk",
            Password = "kj9203KKJHSD?23",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IsVerified.Should().BeFalse();
        A.CallTo(()=> _fakeUserRepository.GetByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._, A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_Given_Valid_Credentials_Should_Return_Result_Ok_With_LoginDto()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("Test@mail.dk"), Password.Create("kj9203KKJHSD?23"), true);

        A.CallTo(() => _fakeUserRepository.GetByEmailAsync(A<string>._)).Returns(user);

        var cmd = new LoginCommand()
        {
            Email = "Test@mail.dk",
            Password = "kj9203KKJHSD?23",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IsVerified.Should().BeTrue();
        A.CallTo(()=> _fakeUserRepository.GetByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._,  A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_Given_Exception_Should_Return_UnspecifiedError()
    {
        // Arrange
        A.CallTo(() => _fakeUserRepository.GetByEmailAsync(A<string>._)).Throws<Exception>();

        var cmd = new LoginCommand()
        {
            Email = "Test@mail.dk",
            Password = "kj9203KKJHSD?23",
            Base64DevicePublicKey = "TestPublicKey",
            DeviceAppVersion = "1.0.0",
            DeviceType = DeviceType.Desktop,
            DevicePlatform = DevicePlatform.Windows
        };

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occured while attempting to login");
        A.CallTo(()=> _fakeUserRepository.GetByEmailAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeDeviceService.RegisterDevice(A<User>._, A<string>._, A<string>._, A<DeviceType>._, A<DevicePlatform>._)).MustNotHaveHappened();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
    }


}