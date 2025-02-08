﻿using Cypherly.ChatServer.Application.Cache.Client;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;
using Cypherly.ChatServer.Domain.Aggregates;
using Cypherly.Domain.Common;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cypherly.ChatServer.Application.Test.Unit.CommandTest.DisconnectClientCommandTest;

public class DisconnectClientCommandHandlerTest
{
    private readonly IClientCache _fakeCache;
    private readonly DisconnectClientCommandHandler _sut;

    public DisconnectClientCommandHandlerTest()
    {
        _fakeCache = A.Fake<IClientCache>();
        _sut = new DisconnectClientCommandHandler(_fakeCache, A.Fake<ILogger<DisconnectClientCommandHandler>>());
    }

    [Fact]
    public async Task Handle_Client_Not_In_Cache_Should_Return_Result_Fail()
    {
        // Arrange
        const string transientId = "test_transient_id";

        A.CallTo(() => _fakeCache.GetByTransientIdAsync(transientId, A<CancellationToken>._)).Returns<ClientCacheDto?>(null);

        var command = new DisconnectClientCommand { TransientId = transientId };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _fakeCache.GetByTransientIdAsync(transientId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeCache.RemoveAsync(A<Guid>._, A<CancellationToken>._)).MustNotHaveHappened();
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Client_In_Cache_Should_Return_Result_Ok()
    {
        // Arrange
        const string transientId = "test_transient_id";
        var client = new Client(Guid.NewGuid(), Guid.NewGuid());
        var clientCacheDto = ClientCacheDto.Create(client, transientId);

        A.CallTo(() => _fakeCache.GetByTransientIdAsync(transientId, A<CancellationToken>._)).Returns(clientCacheDto);

        var command = new DisconnectClientCommand { TransientId = transientId };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _fakeCache.GetByTransientIdAsync(transientId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeCache.RemoveAsync(clientCacheDto.ConnectionId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Exception_Should_Return_Result_Fail()
    {
        // Arrange
        const string transientId = "test_transient_id";
        var client = new Client(Guid.NewGuid(), Guid.NewGuid());
        var clientCacheDto = ClientCacheDto.Create(client, transientId);

        A.CallTo(() => _fakeCache.GetByTransientIdAsync(transientId, A<CancellationToken>._)).Returns(clientCacheDto);
        A.CallTo(() => _fakeCache.RemoveAsync(A<Guid>._, A<CancellationToken>._)).Throws<Exception>();

        var command = new DisconnectClientCommand { TransientId = transientId };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _fakeCache.GetByTransientIdAsync(transientId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeCache.RemoveAsync(clientCacheDto.ConnectionId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        result.Success.Should().BeFalse();
    }
}
