using System.Reflection;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.Outboxing.Messaging.Configuration;
using Cypherly.UserManagement.Application.Configuration;
using Cypherly.UserManagement.Domain.Configuration;
using Cypherly.UserManagement.Persistence.Configuration;
using Cypherly.UserManagement.Storage.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

var env = builder.Environment;

var configuration = builder.Configuration;
configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();

if (env.IsDevelopment())
{
    configuration.AddJsonFile($"appsettings.{Environments.Development}.json", true, true);
    configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
}
#endregion

#region Logger

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region Domain Layer
builder.Services.AddUserManagementDomainServices();
#endregion

#region Application Layer
builder.Services.AddUserManagementApplication(Assembly.Load("Cypherly.UserManagement.Application"));
#endregion

#region Persistence Layer
builder.Services.AddUserManagementPersistence(configuration);
#endregion

#region Storage

builder.Services.Configure<MinioSettings>(configuration.GetSection("Bucket"));
builder.Services.AddStorage(configuration);

#endregion

#region Outboxing

builder.Services.AddOutboxProcessingJob(Assembly.Load("Cypherly.UserManagement.Application"));

#endregion

#region MassTransit

builder.Services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
builder.Services.AddMassTransitWithRabbitMq(Assembly.Load("Cypherly.UserManagement.Application"));

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program {}