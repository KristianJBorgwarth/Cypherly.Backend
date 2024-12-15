using System.Reflection;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.SagaOrchestrator.Messaging.Data.Context;
using Cypherly.SagaOrchestrator.Messaging.Saga.User.Delete;
using MassTransit;
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
            services.AddDbContext<OrchestratorDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("SagaOrchestratorDbConnectionString")!,
                    b => b.MigrationsAssembly(typeof(OrchestratorDbContext).Assembly.FullName));
            });
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
            services.AddMassTransitWithRabbitMq(Assembly.GetExecutingAssembly(), x =>
            {
                x.AddSagaStateMachine<UserDeleteSaga, UserDeleteSagaState>().EntityFrameworkRepository(r =>
                {
                    r.ExistingDbContext<OrchestratorDbContext>();
                    r.UsePostgres();
                });
            });

        })
        .Build();

    using (var scope = host.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OrchestratorDbContext>();
        Log.Information("Looking for pending migrations...");
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            Log.Information("Applying migrations...");
            dbContext.Database.Migrate();
            Log.Information("Migrations applied successfully");
        }
        else
        {
            Log.Information("No pending migrations found");
        }
    }

    Log.Logger.Information("Starting application: SagaOrchestrator.Messaging");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    Console.WriteLine(ex);
}
finally
{
    Log.CloseAndFlush();
}