﻿using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Features.User.Commands.Create;
using Cypherly.Authentication.Architecture.Test.Helpers;
using Cypherly.Domain.Common;
using MediatR;
using NetArchTest.Rules;

namespace Cypherly.Authentication.Architecture.Test;

public class ApplicationTest
{
    [Fact]
    public void Application_Should_Not_Reference_Infrastructure()
    {
        var result = Types.InAssembly(typeof(CreateUserCommandHandler).Assembly)
            .That()
            .ResideInNamespace("Cypherly.ChatUser.Application")
            .ShouldNot()
            .HaveDependencyOn("Cypherly.ChatUser.Infrastructure")
            .GetResult();

        result.ShouldBeSuccessful("Application project should not reference Infrastructure project");
    }

    [Fact]
    public void Application_Should_Not_Reference_Presentation()
    {
        var result = Types.InAssembly(typeof(CreateUserCommandHandler).Assembly)
            .That()
            .ResideInNamespace("Cypherly.ChatUser.Application")
            .ShouldNot()
            .HaveDependencyOn("Cypherly.ChatUser.API")
            .GetResult();

        result.ShouldBeSuccessful("Application project should not reference Presentation project");
    }

    [Fact]
    public void All_Repositories_Should_Inherit_From_IRepository()
    {
        var result = Types.InAssembly(typeof(IUserRepository).Assembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .HaveDependencyOn("Cypherly.Application.Contracts")
            .GetResult();

        result.ShouldBeSuccessful("All repositories should inherit from IRepository<T>");
    }

    [Fact]
    public void All_Repositories_Should_Reference_Domain()
    {
        var result = Types.InAssembly(typeof(IUserRepository).Assembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .HaveDependencyOn("Cypherly.Authentication.Domain")
            .GetResult();

        result.ShouldBeSuccessful("All repositories should reference Domain project");
    }

    [Fact]
    public void All_Commands_Should_Implement_ICommand()
    {
        var result = Types.InAssembly(typeof(CreateUserCommand).Assembly)
            .That()
            .HaveNameEndingWith("Command")
            .Should()
            .ImplementInterface(typeof(ICommand))
            .Or()
            .ImplementInterface(typeof(ICommand<>))
            .GetResult();

        result.ShouldBeSuccessful("All commands or queries should implement ICommand<T> or ICommand");
    }

    [Fact]
    public void All_Queries_Should_Implement_IQuery()
    {
        var result = Types.InAssembly(typeof(CreateUserCommand).Assembly)
            .That()
            .HaveNameEndingWith("Query")
            .Should()
            .ImplementInterface(typeof(IQuery<>))
            .GetResult();

        result.ShouldBeSuccessful("All commands or queries should implement IQuery<T>");
    }

    [Fact]
    public void All_CommandHandlers_Should_Implement_ICommandHandler()
    {
        var result = Types.InAssembly(typeof(CreateUserCommand).Assembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .GetResult();

        result.ShouldBeSuccessful("All command handlers or query handlers should implement ICommandHandler<>");
    }


}