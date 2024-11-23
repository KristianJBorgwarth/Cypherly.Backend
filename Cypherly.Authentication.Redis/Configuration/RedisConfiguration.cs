using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Authentication.Redis.Configuration;

public static class RedisConfiguration
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");

            //Prepend any key stored from this service with the service name
            options.InstanceName = "Cypherly.Authentication.API_";
        });

        services.AddSingleton<IRedisCacheService, RedisCacheService>();
        services.AddScoped<INonceCacheService, NonceCacheService>();

        return services;
    }
}