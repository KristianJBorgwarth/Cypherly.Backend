using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Valkey.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cypherly.ChatServer.Valkey.Configuration;

public static class ValkeyConfiguration
{
    public static IServiceCollection AddValkey(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var valkeySettings = serviceProvider.GetRequiredService<IOptions<ValkeySettings>>().Value;

            // Construct the connection string from ValkeySettings
            options.Configuration = $"{valkeySettings.Host}:{valkeySettings.Port}";
            options.InstanceName = "Cypherly.Authentication.API_";
        });

        services.AddSingleton<IValkeyCacheService, ValkeyCacheService>();
        services.AddScoped<IClientCache, ClientCache>();

        return services;
    }
}