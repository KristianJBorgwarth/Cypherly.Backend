using System.Reflection;
using Cypherly.Persistence.Configuration;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Persistence.Context;
using Cypherly.UserManagement.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Persistence.Configuration;

public static class UserManagementPersistenceConfiguration
{
    private const string ConnectionStringName = "UserManagementDbConnectionString";

    public static void AddUserManagementPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence<UserManagementDbContext>(configuration, Assembly.GetExecutingAssembly(), ConnectionStringName);

        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
    }
}