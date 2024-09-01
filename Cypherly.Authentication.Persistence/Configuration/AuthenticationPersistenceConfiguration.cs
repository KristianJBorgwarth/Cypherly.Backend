using System.Reflection;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Persistence.Context;
using Cypherly.Authentication.Persistence.Repositories;
using Cypherly.Persistence.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Authentication.Persistence.Configuration;

public static class AuthenticationPersistenceConfiguration
{
    private const string ConnectionStringName = "AuthenticationDbConnectionString";
    public static void AddAuthenticationPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence<AuthenticationDbContext>(configuration, Assembly.GetExecutingAssembly(), ConnectionStringName);

        services.AddScoped<IUserRepository, UserRepository>();
    }
}