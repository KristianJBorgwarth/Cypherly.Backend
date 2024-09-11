using Cypherly.UserManagement.Persistence.Context;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Application.Test.Integration.Setup;

[Collection("UserManagementApplication")]
public class IntegrationTestBase : IDisposable
{
    protected readonly UserManagementDbContext Db;
    protected readonly HttpClient Client;
    protected readonly ITestHarness Harness;

    public IntegrationTestBase(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    {
        Harness = factory.Services.GetTestHarness();
        var scope = factory.Services.CreateScope();
        Db = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
        Db.Database.EnsureCreated();
        Client = factory.CreateClient();
        Harness.Start();
    }

    public void Dispose()
    {
        Db.UserProfile.ExecuteDelete();
    }
}