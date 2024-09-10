using Cypherly.Authentication.Persistence.Context;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Authentication.Application.Test.Integration.Setup;

[Collection("AuthenticationApplication")]
public class IntegrationTestBase : IDisposable
{
    protected readonly AuthenticationDbContext Db;
    protected readonly HttpClient Client;
    protected readonly ITestHarness Harness;

    public IntegrationTestBase(IntegrationTestFactory<Program, AuthenticationDbContext> factory)
    {
        Harness = factory.Services.GetTestHarness();
        var scope = factory.Services.CreateScope();
        Db = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
        Db.Database.EnsureCreated();
        Client = factory.CreateClient();
        Harness.Start();
    }

    public void Dispose()
    {
        Db.VerificationCode.ExecuteDelete();
        Db.User.ExecuteDelete();
        Db.OutboxMessage.ExecuteDelete();
        Harness.Stop();
    }
}