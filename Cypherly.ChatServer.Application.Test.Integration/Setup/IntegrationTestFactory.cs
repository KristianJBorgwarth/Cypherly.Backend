using Cypherly.ChatServer.Valkey.Configuration;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestUtilities;

namespace Cypherly.ChatServer.Application.Test.Integration.Setup;

public class IntegrationTestFactory<TProgram, TDbContext> : BaseIntegrationTestFactory<TProgram, TDbContext>
    where TProgram : class
    where TDbContext : DbContext
{
    private const int ValkeyPort = 6976;

    private readonly IContainer _valkeyContainer = new ContainerBuilder()
        .WithImage("valkey/valkey:latest")
        .WithEnvironment("ALLOW_EMPTY_PASSWORD", "yes")
        .WithExposedPort(ValkeyPort)
        .WithPortBinding(ValkeyPort, 6379)
        .WithCleanUp(true)
        .Build();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        return builder.Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ShouldTestWithLazyLoadingProxies = false;
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {

            #region RabbitMq Configuration

            var rmqDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (rmqDescriptor is not null)
                services.Remove(rmqDescriptor);

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumers(typeof(TProgram).Assembly);
            });

            #endregion

            #region Valkey Configuration

            services.Configure<ValkeySettings>(options =>
            {
                options.Host = "localhost"; // The test container's host
                options.Port = ValkeyPort; // The mapped port for the Valkey container
            });

            #endregion

        });
    }

    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _valkeyContainer.StartAsync();
    }

    public async override Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _valkeyContainer.StopAsync();
    }
}