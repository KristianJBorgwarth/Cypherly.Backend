using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using Cypherly.ChatServer.API.Handlers;
using Cypherly.ChatServer.API.Hubs;
using Cypherly.ChatServer.Application.Configuration;
using Cypherly.ChatServer.Application.Contracts;
using Cypherly.ChatServer.Persistence.Configuration;
using Cypherly.ChatServer.Persistence.Context;
using Cypherly.ChatServer.Valkey.Configuration;
using Cypherly.Common.Messaging.Messages.PublishMessages.Client;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.Persistence.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;

#pragma warning disable CS0618 // Type or member is obsolete


var builder = WebApplication.CreateBuilder(args);

#region Configuration

var env = builder.Environment;

var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();

if (env.IsDevelopment())
{
    configuration.AddJsonFile($"appsettings.{Environments.Development}.json", true, true);
    configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
}

#endregion

#region MassTransit

builder.Services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
builder.Services.AddMassTransitWithRabbitMq(Assembly.Load("Cypherly.ChatServer.Application"))
    .AddProducer<ClientConnectedMessage>();

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

#region ConnectionMultiplexer

builder.Services.Configure<ValkeySettings>(configuration.GetSection("Valkey"));
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var valkeySettings = provider.GetRequiredService<IOptions<ValkeySettings>>().Value;
    var config = new ConfigurationOptions
    {
        AbortOnConnectFail = false,
        EndPoints = { $"{valkeySettings.Host}:{valkeySettings.Port}" },
        ChannelPrefix = "Cypherly.ChatServer.API_",
    };

    var multiplexer = ConnectionMultiplexer.Connect(config);

    multiplexer.ConnectionFailed += (_, e) =>
        Log.Warning("Connection to Valkey failed: {Exception}", e.Exception);

    multiplexer.ConnectionRestored += (_, e) =>
        Log.Information("Connection to Valkey restored");

    Log.Information("Connected to Valkey");
    return multiplexer;
});
#endregion

#region Caching

builder.Services.AddValkey();

#endregion

#region SignalR Configuration

var valkeyHost = configuration["Valkey:Host"];
var valkeyPort = configuration["Valkey:Port"];
Log.Information("SignalR backplane using Valkey at {Host}:{Port}", valkeyHost, valkeyPort);

builder.Services.AddSignalR()
    .AddStackExchangeRedis(options =>
    {;
        options.ConnectionFactory = async _ =>
            await Task.FromResult(builder.Services.BuildServiceProvider()
                .GetRequiredService<IConnectionMultiplexer>());
    });

builder.Services.AddScoped<IChangeEventNotifier, ChangeEventHandler>();
#endregion

#region Authentication & Authorization

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"] ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {configuration["Jwt:Issuer"]}"),
        ValidAudience = configuration["Jwt:Audience"] ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {configuration["Jwt:Audience"]}"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"] ??
                                                                           throw new NotImplementedException("MISSING VALUE IN JWT SETTINGS Jwt:Secret"))),
    };

    options.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/hubs") ||
                 path.StartsWithSegments("/change-eventhub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            Log.Information("Successfully validated token for: {User}",
                context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Log.Information("Authentication failed: {Exception}",
                context.Exception);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowElectron", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
            .AllowAnyMethod()
            .AllowCredentials()
            .AllowAnyHeader();
    });
});

#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (env.IsProduction())
{
    app.Services.ApplyPendingMigrations<ChatServerDbContext>();
}

app.UseCors("AllowElectron");

app.UseAuthentication();

app.UseAuthorization();

Log.Information("Chat Server booted up");

app.MapHub<ChangeEventHub>("change-eventhub");

app.Run();

public partial class Program { }