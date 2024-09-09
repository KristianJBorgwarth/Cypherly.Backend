using Cypherly.Authentication.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Authentication.Application.Test.Integration.Setup;

[Collection("AuthenticationApplication")]
public class IntegrationTestBase : IDisposable
{
    protected readonly AuthenticationDbContext Db;
    protected readonly HttpClient Client;

    public IntegrationTestBase(IntegrationTestFactory<Program, AuthenticationDbContext> factory)
    {
        var scope = factory.Services.CreateScope();
        Db = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
        Db.Database.EnsureCreated();
        Client = factory.CreateClient();
    }

    public void Dispose()
    {
        Db.VerificationCode.ExecuteDelete();
        Db.User.ExecuteDelete();
    }
}