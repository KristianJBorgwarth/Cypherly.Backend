using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Update.VerifyDevice;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.UserTest.CommandTest.UpdateTest.VerifyDeviceTest;

public class VerifyDeviceCommandHandlerTest
{
    private readonly IUserRepository _fakeUserRepository;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly IDeviceService _fakeDeviceService;
    private readonly VerifyDeviceCommandHandler _sut;

    public VerifyDeviceCommandHandlerTest()
    {
        _fakeUserRepository = A.Fake<IUserRepository>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _fakeDeviceService = A.Fake<IDeviceService>();

        _sut = new VerifyDeviceCommandHandler(_fakeUserRepository, _fakeDeviceService, _fakeUnitOfWork, A.Fake<ILogger<VerifyDeviceCommandValidator>>());
    }

    [Fact]
    public async void Handle_Given_valid_Command_Should_Return_ResultOk()
    {
        // Arrange

        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test2kasjda??KK"), true);
        var device = new Device(Guid.NewGuid(), "Test.PC", "genericshit", "1.2", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        user.AddDevice(device);

        var command = new VerifyDeviceCommand
        {
            UserId = user.Id,
            DeviceId = device.Id,
            DeviceVerificationCode = "1234"
        };
        A.CallTo(() => _fakeUserRepository.GetByIdAsync(command.UserId)).Returns(user);
        A.CallTo(()=> _fakeDeviceService.VerifyDevice(user, command.DeviceId, command.DeviceVerificationCode)).Returns(Result.Ok());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        A.CallTo(()=> _fakeUserRepository.UpdateAsync(user)).MustHaveHappenedOnceExactly();
        A.CallTo(()=> _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_Given_User_Is_Null_Should_Return_ResultFail()
    {
        // Arrange
        var command = new VerifyDeviceCommand
        {
            UserId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            DeviceVerificationCode = "1234"
        };

        A.CallTo(() => _fakeUserRepository.GetByIdAsync(command.UserId)).Returns((User)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Could not find entity with ID");
    }

    [Fact]
    public async void Handle_Given_VerifyDeviceResult_Success_Is_False_Should_Return_ResultFail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test2kasjda??KK"), true);
        var device = new Device(Guid.NewGuid(), "Test.PC", "genericshit", "1.2", DeviceType.Desktop, DevicePlatform.Windows, user.Id);

        user.AddDevice(device);

        var command = new VerifyDeviceCommand
        {
            UserId = user.Id,
            DeviceId = device.Id,
            DeviceVerificationCode = "1234"
        };

        A.CallTo(() => _fakeUserRepository.GetByIdAsync(command.UserId)).Returns(user);
        A.CallTo(()=> _fakeDeviceService.VerifyDevice(user, command.DeviceId, command.DeviceVerificationCode)).Returns(Result.Fail(Errors.General.UnspecifiedError("Test")));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Test");
    }

    [Fact]
    public async void Handle_Given_Something_Throws_Exception_Should_Return_Result_Fail()
    {
        // Arrange
        var command = new VerifyDeviceCommand
        {
            UserId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            DeviceVerificationCode = "1234"
        };

        A.CallTo(() => _fakeUserRepository.GetByIdAsync(command.UserId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An Exception occurred while verifying device.");
    }

}