using System.Reflection;
using System.Text;
using Cypherly.API.Filters;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.MassTransit.Messaging.Configuration;
using Cypherly.Outboxing.Messaging.Configuration;
using Cypherly.UserManagement.Application.Configuration;
using Cypherly.UserManagement.Domain.Configuration;
using Cypherly.UserManagement.Persistence.Configuration;
using Cypherly.UserManagement.Storage.Configuration;
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
builder.Services.AddMassTransitWithRabbitMq(Assembly.Load("Cypherly.UserManagement.Application"))
    .AddProducer<OperationSuccededMessage>();

#endregion

#region Authentication & Authorization

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"] ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {configuration["Jwt:Issuer"]}"),
            ValidAudience = configuration["Jwt:Audience"] ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {configuration["Jwt:Audience"]}"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"] ?? throw new NotImplementedException("MISSING VALUE IN JWT SETTINGS Jwt:Secret")))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
    .AddPolicy("User", policy => policy.RequireRole("user"));

builder.Services.AddScoped<IValidateUserIdFilter, ValidateUserIdIdFilter>();

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

    c.AddSecurityRequirement(new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program {}