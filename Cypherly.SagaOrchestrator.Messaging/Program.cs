using System.Reflection;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.SagaOrchestrator.Messaging.Saga.User.Delete;
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
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
            services.AddMassTransitWithRabbitMq(Assembly.GetExecutingAssembly(), x =>
            {
                x.AddSagaStateMachine<UserDeleteSaga, UserDeleteState>();
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
