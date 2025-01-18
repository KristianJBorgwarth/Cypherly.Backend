using System.Reflection;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Persistence.Context;
using Cypherly.ChatServer.Persistence.Repositories;
using Cypherly.Persistence.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.ChatServer.Persistence.Configuration;

public static class ChatServerPersistenceConfiguration
{
    private const string ConnectionStringName = "ChatServerDbConnectionString";
    
    public static IServiceCollection AddChatServerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence<ChatServerDbContext>(configuration, Assembly.GetExecutingAssembly(),
            ConnectionStringName, false);

        services.AddScoped<IClientRepository, ClientRepository>();
        
        return services;
    }
}