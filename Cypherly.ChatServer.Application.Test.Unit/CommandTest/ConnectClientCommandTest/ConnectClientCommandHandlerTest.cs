﻿using Cypherly.Application.Contracts.Repository;
using Cypherly.ChatServer.Application.Cache.Client;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Application.Features.Client.Commands.Connect;
using Cypherly.ChatServer.Domain.Aggregates;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Cypherly.ChatServer.Application.Test.Unit.CommandTest.ConnectClientCommandTest;

public class ConnectClientCommandHandlerTest
{
    private readonly IClientCache _fakeCache;
    private readonly IClientRepository _fakeRepository;
    private readonly IUnitOfWork _fakeUow;
    private readonly ConnectClientCommandHandler _sut;

    public ConnectClientCommandHandlerTest()
    {
        _fakeCache = A.Fake<IClientCache>();
        _fakeRepository = A.Fake<IClientRepository>();
        _fakeUow = A.Fake<IUnitOfWork>();
        _sut = new ConnectClientCommandHandler(_fakeRepository, _fakeUow, _fakeCache, A.Fake<ILogger<ConnectClientCommandHandler>>());
    }

    [Fact]
    public async Task Handle_Invalid_ClientId_Should_Return_Result_Fail()
    {
        // Arrange
        var clientId = Guid.NewGuid();

        A.CallTo(() => _fakeRepository.GetByIdAsync(clientId)).Returns((Client)null);


        var command = new ConnectClientCommand()
        {
            ClientId = clientId,
            TransientId = "test",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _fakeRepository.GetByIdAsync(clientId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeCache.AddAsync(A<ClientCacheDto>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Valid_ClientId_Should_Return_Result_Ok()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = new Client(Guid.NewGuid(), Guid.NewGuid());

        A.CallTo(() => _fakeRepository.GetByIdAsync(clientId)).Returns(client);

        var command = new ConnectClientCommand()
        {
            ClientId = clientId,
            TransientId = "test",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _fakeRepository.GetByIdAsync(clientId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeCache.AddAsync(A<ClientCacheDto>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Exception_Should_Return_Result_Fail()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = new Client(Guid.NewGuid(), Guid.NewGuid());

        A.CallTo(() => _fakeRepository.GetByIdAsync(clientId)).Returns(client);
        A.CallTo(() => _fakeCache.AddAsync(A<ClientCacheDto>._, A<CancellationToken>._)).Throws<Exception>();

        var command = new ConnectClientCommand()
        {
            ClientId = clientId,
            TransientId = "test",
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _fakeRepository.GetByIdAsync(clientId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeCache.AddAsync(A<ClientCacheDto>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
        result.Success.Should().BeFalse();
    }
}