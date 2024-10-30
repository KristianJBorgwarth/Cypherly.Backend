using System.Reflection;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.SagaOrchestrator.Messaging.Data.Context;
using Cypherly.SagaOrchestrator.Messaging.Saga.User.Delete;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    var host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((builder) =>
        {
            builder.AddConfiguration(configuration);
        })
        .UseSerilog()
        .ConfigureServices((ctx, services) =>
        {
            services.AddDbContext<SagaDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("SagaOrchestratorDbConnectionString")!, b=> b.MigrationsAssembly(typeof(OrchestratorDbContext).Assembly.FullName));
            });
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
            services.AddMassTransitWithRabbitMq(Assembly.GetExecutingAssembly(), x =>
            {
                x.AddSagaStateMachine<UserDeleteSaga, UserDeleteSagaState>().EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<SagaDbContext>();
                    r.UsePostgres();
                });
            });

        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
