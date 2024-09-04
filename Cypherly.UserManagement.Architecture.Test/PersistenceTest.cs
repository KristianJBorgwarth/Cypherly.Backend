using Cypherly.Persistence.Context;
using Cypherly.UserManagement.Architecture.Test.Helpers;
using Cypherly.UserManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;

namespace Cypherly.UserManagement.Architecture.Test;

public class PersistenceTest
{
     [Fact]
    public void Infrastructure_Should_Not_Reference_Presentation()
    {
        var result = Types.InAssembly(typeof(UserManagementDbContext).Assembly)
            .That()
            .ResideInNamespace("Cypherly.UserManagement.Persistence")
            .ShouldNot()
            .HaveDependencyOn("Cypherly.UserManagement.API")
            .GetResult();

        result.ShouldBeSuccessful("Infrastructure project should not reference Presentation project");
    }

    [Fact]
    public void Infrastructure_Should_Not_Reference_Application_Outside_Of_Repositories()
    {
        var result = Types.InAssembly(typeof(UserManagementDbContext).Assembly)
            .That()
            .ResideInNamespace("Cypherly.Authentication.Persistence")
            .And()
            .DoNotResideInNamespace("Cypherly.UserManagement.Persistence.Configuration")
            .And()
            .DoNotResideInNamespace("Cypherly.UserManagement.Persistence.Repositories")
            .ShouldNot()
            .HaveDependencyOn("Cypherly.Authentication.Application")
            .GetResult();

        result.ShouldBeSuccessful("Infrastructure project should not reference Application project outside of Repositories and Configration");
    }

    [Fact]
    public void All_Repositories_Should_Reference_Domain()
    {
        var result = Types.InAssembly(typeof(UserManagementDbContext).Assembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .HaveDependencyOn("Cypherly.UserManagement.Domain")
            .GetResult();

        result.ShouldBeSuccessful("All repositories should reference Domain project");
    }

    [Fact]
    public void All_Repositories_Should_Reference_Infrastructure_Context()
    {
        var result = Types.InAssembly(typeof(UserManagementDbContext).Assembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .HaveDependencyOn("Cypherly.UserManagement.Persistence.Context")
            .GetResult();

        result.ShouldBeSuccessful("All repositories should reference Infrastructure Context project");
    }

    [Fact]
    public void All_Contexts_Should_Inherit_From_CypherlyDbContext()
    {
        var result = Types.InAssembly(typeof(UserManagementDbContext).Assembly)
            .That()
            .HaveNameEndingWith("DbContext")
            .And().AreClasses()
            .Should()
            .Inherit(typeof(CypherlyBaseDbContext))
            .GetResult();

        result.ShouldBeSuccessful("All contexts should inherit from CypherlyDbContext");
    }

    [Fact]
    public void All_ModelConfigurations_Should_Inherit_From_IEntityTypeConfiguration()
    {
        var result = Types.InAssembly(typeof(UserManagementDbContext).Assembly)
            .That()
            .AreClasses()
            .And()
            .ResideInNamespace("Cypherly.Authentication.Persistence.ModelConfiguration")
            .Should()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .GetResult();

        result.ShouldBeSuccessful("All ModelConfigurations should inherit from IEntityTypeConfiguration");
    }
}