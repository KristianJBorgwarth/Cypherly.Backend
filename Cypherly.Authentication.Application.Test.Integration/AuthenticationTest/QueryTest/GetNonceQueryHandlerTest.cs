﻿using System.Text.Json;
using Cypherly.Authentication.Application.Caching;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.Authentication.Queries.GetNonce;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.AuthenticationTest.QueryTest;

public class GetNonceQueryHandlerTest : IntegrationTestBase
{
    private readonly GetNonceQueryHandler _sut;
    public GetNonceQueryHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var cache = scope.ServiceProvider.GetRequiredService<INonceCacheService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetNonceQueryHandler>>();

        _sut = new GetNonceQueryHandler(repo, cache, logger);
    }

    [Fact]
    public async void Handle_WhenQueryValid_Should_Generate_Nonce_And_Cache_And_Return()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kjsidlæ??238Ja"), true);
        var device = new Device(Guid.NewGuid(), "TestDevice", "SomeKey", "1.0", DeviceType.Desktop,
            DevicePlatform.Android, user.Id);
        user.AddDevice(device);
        Db.User.Add(user);
        await Db.SaveChangesAsync();

        var query = new GetNonceQuery()
        {
            UserId = user.Id,
            DeviceId = device.Id
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        var options = new JsonSerializerOptions()
        {
            Converters = { new NonceJsonConverter() },
        };
        var nonce = await Cache.GetAsync<Nonce>(result.Value!.Id.ToString(), options, new CancellationToken());
        nonce.Should().NotBeNull();
        nonce!.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async void Handle_WhenQueryInvalid_UserNot_Found_Should_Not_Cachce_And_Return_Result_Fail()
    {
        // Arrange
        var query = new GetNonceQuery()
        {
            UserId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid()
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Contain("Could not find entity with ID");
    }
}