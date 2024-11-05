﻿using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Delete;
using Cypherly.Authentication.Application.Test.Integration.Setup;
using Cypherly.Authentication.Domain.Aggregates;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Authentication.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Test.Integration.UserTest.CommandTest.DeleteTest;

public class DeleteUserCommandHandlerTest : IntegrationTestBase
{
    private readonly DeleteUserCommandHandler _sut;

    public DeleteUserCommandHandlerTest(IntegrationTestFactory<Program, AuthenticationDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userLifeCycleServices = scope.ServiceProvider.GetRequiredService<IUserLifeCycleServices>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DeleteUserCommandHandler>>();

        _sut = new DeleteUserCommandHandler(repo, unitOfWork, userLifeCycleServices, logger);
    }

    [Fact]
    public async Task Handle_When_Command_Is_Valid_Should_Delete_User()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kajshas=?2323S"), false);
        await Db.AddAsync(user);
        await Db.SaveChangesAsync();

        var cmd = new DeleteUserCommand()
        {
            Id = user.Id
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        var deletedUser = await Db.User.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
        deletedUser.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_When_User_Is_Not_Found_Should_Return_NotFound()
    {
        // Arrange
        var cmd = new DeleteUserCommand()
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("Could not find entity with ID");
    }

    [Fact]
    public async Task Handle_When_User_Is_Already_Deleted_Should_Return_UnspecifiedError()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Email.Create("test@mail.dk"), Password.Create("kajshas=?2323S"), false);
        user.SetDelete();
        await Db.AddAsync(user);
        await Db.SaveChangesAsync();

        var cmd = new DeleteUserCommand()
        {
            Id = user.Id
        };

        // Act
        var result = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("User is already marked as deleted");
    }
}