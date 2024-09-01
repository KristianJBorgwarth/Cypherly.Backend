using System.Reflection;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Persistence.Configuration;

public static class PersistenceConfiguration
{
    public static IServiceCollection AddPersistence<TContext>(this IServiceCollection services, IConfiguration configuration, Assembly assembly) where TContext : DbContext
    {
        services.AddDbContext<TContext>(options=>
            options.UseNpgsql(configuration.GetConnectionString("DbConnectionString")!,
                    b => b.MigrationsAssembly(assembly.FullName))
                .UseLazyLoadingProxies());

        services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();

        return services;
    }
}