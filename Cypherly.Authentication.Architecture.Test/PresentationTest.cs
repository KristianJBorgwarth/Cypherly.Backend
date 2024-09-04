﻿using Cypherly.Authentication.Architecture.Test.Helpers;
using Cypherly.Authentication.Controllers;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;

namespace Cypherly.Authentication.Architecture.Test;

public class PresentationTest
{
    [Fact]
    public void Presentation_Should_Not_Reference_Infrastructure()
    {
        var result = Types.InAssembly(typeof(UserController).Assembly)
            .That()
            .ResideInNamespace("Cypherly.Authentication.Controllers")
            .ShouldNot()
            .HaveDependencyOn("Cypherly.Authentication.Persistence")
            .GetResult();

        result.ShouldBeSuccessful("API project should not reference Infrastructure project");
    }

    [Fact]
    public void Presentation_Should_Not_Reference_Domain()
    {
        var result = Types.InAssembly(typeof(UserController).Assembly)
            .That()
            .ResideInNamespace("Cypherly.ChatUser.API.ControllerTest")
            .ShouldNot()
            .HaveDependencyOn("Cypherly.ChatUser.Domain")
            .GetResult();

        result.ShouldBeSuccessful("API project should not reference Domain project");
    }

    [Fact]
    public void Presentation_Should_Reference_Application()
    {
        var result = Types.InAssembly(typeof(UserController).Assembly)
            .That()
            .ResideInNamespace("Cypherly.ChatUser.API.ControllerTest")
            .And()
            .DoNotHaveName("BaseController")
            .Should()
            .HaveDependencyOn("Cypherly.ChatUser.Application")
            .GetResult();

        result.ShouldBeSuccessful("API project should reference Application project");
    }

    [Fact]
    public void All_Controllers_Should_Inherit_From_BaseController()
    {
        var result = Types.InAssembly(typeof(UserController).Assembly)
            .That()
            .ResideInNamespace("Cypherly.ChatUser.API")
            .And()
            .HaveNameEndingWith("ControllerTest")
            .And()
            .DoNotHaveName("BaseController")
            .Should()
            .Inherit(typeof(BaseController))
            .GetResult();

        result.ShouldBeSuccessful("All controllers should inherit from BaseController");
    }
}