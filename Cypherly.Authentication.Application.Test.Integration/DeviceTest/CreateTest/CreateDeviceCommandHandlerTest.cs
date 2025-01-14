using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Caching.LoginNonce;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Device.Commands.Create;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using Cypherly.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.DeviceTest.CreateTest;

public class CreateDeviceCommandHandlerTest : IntegrationTestBase
{
    private readonly CreateDeviceCommandHandler _sut;
    private readonly ILoginNonceCache _loginNonceCache;

    public CreateDeviceCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scoperino = factory.Services.CreateScope();
        var repo = scoperino.ServiceProvider.GetRequiredService<IUserRepository>();
        _loginNonceCache = scoperino.ServiceProvider.GetRequiredService<ILoginNonceCache>();
        var deviceService = scoperino.ServiceProvider.GetRequiredService<IDeviceService>();
        var unitOfWork = scoperino.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scoperino.ServiceProvider.GetRequiredService<ILogger<CreateDeviceCommandHandler>>();
        _sut = new CreateDeviceCommandHandler(repo, _loginNonceCache, deviceService, unitOfWork, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Command_Should_Return_ResultOk_And_Create_Device()
    {
        // Arrange user
        var user = new User(Guid.NewGuid(), Email.Create("test@gmail.com"), Password.Create("test=???KKL999"), true);

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        // Arrange nonce
        var loginNonce = LoginNonce.Create(user.Id);
        await _loginNonceCache.AddNonceAsync(loginNonce, new CancellationToken());

        // Arrange command
        var cmd = new CreateDeviceCommand()
        {
            UserId = user.Id,
            LoginNonceId = loginNonce.Id,
            LoginNonce = loginNonce.NonceValue,
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Mobile,
            DevicePlatform = DevicePlatform.Android,
            Base64DevicePublicKey = "base64DevicePublicKey",
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.DeviceId.Should().NotBeEmpty();
        result.Value.DeviceConnectionId.Should().NotBeEmpty();

        Db.Device.Count().Should().Be(1);
        var device = Db.Device.First();
        device.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_Given_Invalid_UserId_Should_Return_ResultFail()
    {
        // Arrange
        var cmd = new CreateDeviceCommand()
        {
            UserId = Guid.NewGuid(),
            LoginNonceId = Guid.NewGuid(),
            LoginNonce = "nonce",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Mobile,
            DevicePlatform = DevicePlatform.Android,
            Base64DevicePublicKey = "base64DevicePublicKey",
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(cmd.UserId));
    }

    [Fact]
    public async Task Handle_Given_Invalid_LoginNonceId_Should_Return_ResultFail()
    {
        // Arrange user
        var user = new User(Guid.NewGuid(), Email.Create("test@gmail.com"), Password.Create("test=???KKL999"), true);

        Db.User.Add(user);
        await Db.SaveChangesAsync();

        // Arrange nonce
        var loginNonce = LoginNonce.Create(user.Id);
        await _loginNonceCache.AddNonceAsync(loginNonce, new CancellationToken());

        // Arrange command
        var cmd = new CreateDeviceCommand()
        {
            UserId = user.Id,
            LoginNonceId = loginNonce.Id,
            LoginNonce = "invalidNonce",
            DeviceAppVersion = "1.0",
            DeviceType = DeviceType.Mobile,
            DevicePlatform = DevicePlatform.Android,
            Base64DevicePublicKey = "base64DevicePublicKey",
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.Unauthorized());
    }
}