using System.Reflection;
using Cypherly.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.ChatServer.Application.Configuration;

public static class ChatServerApplicationConfiguration 
{
    public static void AddChatServerApplication(this IServiceCollection services, Assembly assembly)
    {
        services.AddApplication(assembly);
    }
}