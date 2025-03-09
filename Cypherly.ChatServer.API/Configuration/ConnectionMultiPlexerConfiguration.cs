using Cypherly.ChatServer.Valkey.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;

namespace Cypherly.ChatServer.API.Configuration;

public static class ConnectionMultiPlexerConfiguration
{
    public static IServiceCollection AddConnectionMultiplexer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ValkeySettings>(configuration.GetSection("Valkey"));

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var valkeySettings = provider.GetRequiredService<IOptions<ValkeySettings>>().Value;
            var config = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints = { $"{valkeySettings.Host}:{valkeySettings.Port}" },
                ChannelPrefix = "Cypherly.ChatServer.API_",
            };
            var multiplexer = ConnectionMultiplexer.Connect(config);

            multiplexer.ConnectionFailed += (_, e) =>
                Log.Warning("Connection to Valkey failed: {Exception}", e.Exception);

            multiplexer.ConnectionRestored += (_, e) =>
                Log.Information("Connection to Valkey restored");

            Log.Information("Connected to Valkey");
            return multiplexer;
        });

        return services;
    }
}