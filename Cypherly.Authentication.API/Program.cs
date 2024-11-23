using System.Reflection;
using System.Text;
using Cypherly.Authentication.Application.Configuration;
using Cypherly.Authentication.Application.Features.User.Consumers;
using Cypherly.Authentication.Application.Services.Authentication;
using Cypherly.Authentication.Domain.Configuration;
using Cypherly.Authentication.Persistence.Configuration;
using Cypherly.Authentication.Redis.Configuration;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.Common.Messaging.Messages.PublishMessages.Email;
using Cypherly.Common.Messaging.Messages.PublishMessages.User.Delete;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.Outboxing.Messaging.Configuration;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

builder.Services.AddAuthenticationDomain();

#endregion

#region Application Layer

builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
builder.Services.AddAuthenticationApplication(Assembly.Load("Cypherly.Authentication.Application"));

#endregion

#region Persistence Layer

builder.Services.AddAuthenticationPersistence(configuration);

#endregion

#region Outboxing

builder.Services.AddOutboxProcessingJob(Assembly.Load("Cypherly.Authentication.Application"));

#endregion

#region MassTransit

builder.Services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
builder.Services.AddMassTransitWithRabbitMq(Assembly.Load("Cypherly.Authentication.Application"), null, (cfg, context)=>
    {
        cfg.ReceiveEndpoint("authentication_fail_queue", e=>
        {
            e.Consumer<RollbackUserDeleteConsumer>(context);
        });
    })
    .AddProducer<SendEmailMessage>()
    .AddProducer<UserDeletedMessage>()
    .AddProducer<OperationSuccededMessage>();

#endregion

#region Redis

builder.Services.AddRedis(configuration);

#endregion


#region Authenticaion & Authorization
var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        if(jwtSettings is null)
            throw new NotImplementedException("MISSING JWT SETTINGS");
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {jwtSettings.Issuer}"),
            ValidAudience = jwtSettings.Audience ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {jwtSettings.Audience}"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
    .AddPolicy("User", policy => policy.RequireRole("user"));

#endregion

#region  CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowElectron", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:8080");
    });
});

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Cypherly.Authentication.API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowElectron");
app.Run();

public partial class Program { }