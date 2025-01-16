using System.Runtime.InteropServices.JavaScript;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Device.Queries.GetDevices;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Unit.DeviceTest.QueryTest.GetDevicesTest;

public class GetDevicesQueryHandlerTest
{
    private readonly IUserRepository _fakeUserRepository;
    private readonly GetDevicesQueryHandler _sut;

    public GetDevicesQueryHandlerTest()
    {
        _fakeUserRepository = A.Fake<IUserRepository>();
        var logger = A.Fake<ILogger<GetDevicesQueryHandler>>();
        _sut = new GetDevicesQueryHandler(_fakeUserRepository, logger);
    }

    [Fact]
    public async Task Handle_GivenADevice_ShouldReturnAListOfDevices()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test??kl98K"), true);
        
        var device = new Device(Guid.NewGuid(), "someKey", "1.0", DeviceType.Desktop, DevicePlatform.Android, user.Id);
        
        user.AddDevice(device);

        var query = new GetDevicesQuery()
        {
            UserId = user.Id
        };
        
        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(query.UserId)).Returns(user);
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Devices.Should().NotBeNull().And.NotBeEmpty();
        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(query.UserId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_GivenNoDevices_Should_Return_Empty_Result()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("test??kl98K"), true);
        
        var query = new GetDevicesQuery()
        {
            UserId = user.Id
        };
        
        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(query.UserId)).Returns(user);
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Devices.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Handle_Given_Invalid_UserId_Should_Return_Fail()
    {
        // Arrange
        var query = new GetDevicesQuery()
        {
            UserId = Guid.NewGuid()
        };
        
        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(query.UserId)).Returns((User)null);
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull().And.BeEquivalentTo(Errors.General.NotFound(query.UserId));
    }

    [Fact]
    public async Task Handle_Given_Something_Throws_Exception_Should_Return_Fail()
    {
        // Arrange
        var query = new GetDevicesQuery()
        {
            UserId = Guid.NewGuid()
        };
        
        A.CallTo(()=> _fakeUserRepository.GetByIdAsync(query.UserId)).Throws<Exception>();
        
        // Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        
        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull().And.BeEquivalentTo(Errors.General.UnspecifiedError("An exception occured while fetching devices"));
    }
}