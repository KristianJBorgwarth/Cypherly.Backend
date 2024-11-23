using Cypherly.UserManagement.Application.Test.Integration.Setup.Helpers;
using Cypherly.UserManagement.Bucket.Configuration;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TestUtilities;

// ReSharper disable ClassNeverInstantiated.Global

namespace Cypherly.UserManagement.Application.Test.Integration.Setup;

public class IntegrationTestFactory<TProgram, TDbContext> : BaseIntegrationTestFactory<TProgram, TDbContext>
    where TProgram : class
    where TDbContext : DbContext
{
    private readonly IContainer _minioBucketContainer = new ContainerBuilder()
        .WithImage("bitnami/minio:latest")
        .WithEnvironment("MINIO_ROOT_USER", "MinioRoot")
        .WithEnvironment("MINIO_ROOT_PASSWORD", "rootErinoTest?87")
        .WithExposedPort(9000)
        .WithExposedPort(9001)
        .WithPortBinding(9023, 9000)
        .WithPortBinding(9024, 9001)
        .WithCleanUp(true)
        .Build();

    private readonly MinioBucketHandler _minioBucketHandler = new("MinioRoot", "rootErinoTest?87");


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            #region RabbitMq Configuration

            var rmgDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBusControl));

            if (rmgDescriptor is not null)
                services.Remove(rmgDescriptor);

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumers(typeof(TProgram).Assembly);
            });

            #endregion

            #region Minio Configuration


            services.RemoveAll(typeof(IConfigureOptions<MinioSettings>));

            // Add in-memory configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Bucket:Host", "http://localhost:9023"},
                {"Bucket:ProfilePictureBucket", "bucket-name"},
                {"Bucket:User", "MinioRoot"},
                {"Bucket:Password", "rootErinoTest?87"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            services.Configure<MinioSettings>(configuration.GetSection("Bucket"));

            #endregion
        });

    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _minioBucketContainer.StartAsync();

        // Adding a delay to ensure Minio is ready
        await Task.Delay(10000);
        await _minioBucketHandler.CreateBucketAsync("bucket-name");
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync(); // Dispose of the PostgreSQL container
        await _minioBucketContainer.StopAsync();
    }
}