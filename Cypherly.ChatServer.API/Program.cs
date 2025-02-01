using System.Reflection;
using Cypherly.ChatServer.Application.Configuration;
using Cypherly.ChatServer.Persistence.Configuration;
using Cypherly.ChatServer.Valkey.Configuration;
using Cypherly.MassTransit.Messaging.Configuration;
using MassTransit;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

#region Configuration

var env = builder.Environment;

var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();

if(env.IsDevelopment())
{
    configuration.AddJsonFile($"appsettings.{Environments.Development}.json", true, true);
    configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
}

#endregion

#region MassTransit

builder.Services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
builder.Services.AddMassTransitWithRabbitMq(Assembly.Load("Cypherly.ChatServer.Application"));

#endregion

#region Logger

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region Application Layer

builder.Services.AddChatServerApplication(Assembly.Load("Cypherly.ChatServer.Application"));

#endregion

# region Persistence Layer

builder.Services.AddChatServerPersistence(configuration);

#endregion

#region Caching

builder.Services.AddValkey(configuration);

#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Log.Information("Chat Server booted up");

app.UseHttpsRedirection();

app.Run();