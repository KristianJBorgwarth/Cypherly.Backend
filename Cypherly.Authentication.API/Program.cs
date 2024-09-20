using System.Reflection;
using System.Text;
using Cypherly.Application.Contracts.Messaging.PublishMessages.Email;
using Cypherly.Authentication.Application.Configuration;
using Cypherly.Authentication.Application.Services.Authentication;
using Cypherly.Authentication.Domain.Configuration;
using Cypherly.Authentication.Persistence.Configuration;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.Outboxing.Messaging.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddMassTransitWithRabbitMq(Assembly.Load("Cypherly.Authentication.Application"))
    .AddProducer<SendEmailMessage>();

#endregion

#region Authenticaion 
var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
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
#endregion


builder.Services.AddAuthorization(options =>
{
    // Policy based on a custom claim (e.g., "role" claim)
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("admin"));
    
    options.AddPolicy("User", policy => policy.RequireRole("user"));
});


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

public partial class Program { }