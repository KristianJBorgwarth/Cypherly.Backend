using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Update.VerifyDevice;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.CommandTest.UpdateTest;

public class VerifyDeviceCommandHandlerTest : IntegrationTestBase
{
    private readonly VerifyDeviceCommandHandler _sut;

    public VerifyDeviceCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<VerifyDeviceCommandValidator>>();

        _sut = new VerifyDeviceCommandHandler(repo, deviceService,uow, logger);
    }

    [Fact]
    public async void Handle_WhenUserNotFound_Returns_Result_Fail()
    {
        // Arrange
        var command = new VerifyDeviceCommand()
        {
            UserId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            DeviceVerificationCode = "1234"
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Could not find entity with ID");
    }

    [Fact]
    public async void Handle_WhenDeviceVerificationCodeInvalid_Returns_Result_Fail()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("isksd??lLJK99"), true);
        var device = new Device(Guid.NewGuid(), "test", "publicKey", "1.0", DeviceType.Mobile, DevicePlatform.Android, user.Id);
        device.AddDeviceVerificationCode();
        user.AddDevice(device);

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var command = new VerifyDeviceCommand()
        {
            UserId = user.Id,
            DeviceId = device.Id,
            DeviceVerificationCode = "29313"
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Invalid verification code");
    }

    [Fact]
    public async void Handle_WhenDeviceVerificationCodeValid_Returns_Result_Ok()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("isksd??lLJK99"), true);
        var device = new Device(Guid.NewGuid(), "test", "publicKey", "1.0", DeviceType.Mobile, DevicePlatform.Android, user.Id);
        device.AddDeviceVerificationCode();
        user.AddDevice(device);

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var command = new VerifyDeviceCommand()
        {
            UserId = user.Id,
            DeviceId = device.Id,
            DeviceVerificationCode = device.GetActiveVerificationCode()!.Code.Value
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.Device.AsNoTracking().First().Status.Should().Be(DeviceStatus.Trusted);
        Db.DeviceVerificationCode.AsNoTracking().First().Code.IsUsed.Should().BeTrue();
    }
}