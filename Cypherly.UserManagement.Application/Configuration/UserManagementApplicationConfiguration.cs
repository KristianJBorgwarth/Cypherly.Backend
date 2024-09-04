using System.Reflection;
using Cypherly.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Application.Configuration;

public static class UserManagementApplicationConfiguration
{
    public static void AddUserManagementApplication(this IServiceCollection services, Assembly assembly)
    {
        services.AddApplication(assembly);
    }
}