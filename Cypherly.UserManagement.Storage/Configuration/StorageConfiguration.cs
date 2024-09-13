using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Storage.Configuration;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }
}