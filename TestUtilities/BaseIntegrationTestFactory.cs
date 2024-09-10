using System.ComponentModel;
using Cypherly.Application.Contracts.Messaging.RequestMessages.User.Create;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.Outboxing.Messaging.BackgroundJobs;
using Cypherly.Outboxing.Messaging.Configuration;
using DotNet.Testcontainers.Builders;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Testcontainers.PostgreSql;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace TestUtilities;

public class BaseIntegrationTestFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class where TDbContext : DbContext
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithCleanUp(true)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {

            #region Database Configuration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString(),
                    b => b.MigrationsAssembly(typeof(TDbContext).Assembly.FullName))
                    .UseLazyLoadingProxies();
            });

            #endregion

            #region RabbitMq Configuration

            var rmgDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (rmgDescriptor is not null)
                services.Remove(rmgDescriptor);

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<CreateUserProfileRequest>(async cxt =>
                {
                    await cxt.RespondAsync(new CreateUserProfileResponse());
                });
            });

            #endregion
        });
    }

    public virtual async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new virtual async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}