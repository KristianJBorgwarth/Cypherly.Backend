using System.Reflection;
using Cypherly.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Authentication.Application.Configuration;

public static class AuthenticationApplicationConfiguration
{
    public static void AddAuthenticationApplication(this IServiceCollection services, Assembly assembly)
    {
        services.AddApplication(assembly);
    }
}