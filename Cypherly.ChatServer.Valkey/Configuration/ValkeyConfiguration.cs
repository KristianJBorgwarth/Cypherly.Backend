using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Valkey.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Cypherly.ChatServer.Valkey.Configuration;

public static class ValkeyConfiguration
{
    public static IServiceCollection AddValkey(this IServiceCollection services)
    {
        Console.WriteLine("Adding Valkey configuration");

        services.AddSingleton<IDistributedCache>(provider =>
        {
            var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return new RedisCache(new RedisCacheOptions()
            {
                ConnectionMultiplexerFactory = () => Task.FromResult(multiplexer),
            });
        });

        services.AddSingleton<IValkeyCacheService, ValkeyCacheService>();
        services.AddScoped<IClientCache, ClientCache>();
        return services;
    }
}