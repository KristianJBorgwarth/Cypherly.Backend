using Cypherly.ChatServer.API.Handlers;
using Cypherly.ChatServer.Application.Contracts;
using StackExchange.Redis;

namespace Cypherly.ChatServer.API.Configuration;

public static class SignalRConfiguration
{
    public static IServiceCollection ConfigureSignalR(this IServiceCollection services)
    {
        services.AddSignalR()
            .AddStackExchangeRedis(options =>
            {;
                options.ConnectionFactory = async _ =>
                    await Task.FromResult(services.BuildServiceProvider()
                        .GetRequiredService<IConnectionMultiplexer>());
            });

        services.AddScoped<IChangeEventNotifier, ChangeEventHandler>();
        return services;
    }
}