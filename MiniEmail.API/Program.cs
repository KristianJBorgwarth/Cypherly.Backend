using System.Reflection;
using Cypherly.Common.Messaging.Messages.PublishMessages;
using Cypherly.MassTransit.Messaging.Configuration;
using FluentValidation;
using MinimalEmail.API.Email;
using MinimalEmail.API.Email.Smtp;
using MinimalEmail.API.Features.Requests;
using Smtp_ISmtpClient = MinimalEmail.API.Email.Smtp.ISmtpClient;

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

#region MassTransit

builder.Services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
builder.Services.AddMassTransitWithRabbitMq(Assembly.GetExecutingAssembly()).AddProducer<OperationSuccededMessage>();

#endregion

#region Email Service
builder.Services.Configure<SmtpSettings>(configuration.GetSection("Email:Smtp"));
builder.Services.AddScoped<Smtp_ISmtpClient, SmtpClientWrapper>();
builder.Services.AddScoped<IEmailService, EmailService>();
#endregion

#region Validation

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("send-email", async (SendEmailRequest request, IValidator<SendEmailRequest> validator, IEmailService emailService) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (validationResult.IsValid is false)
        return Results.BadRequest(validationResult.Errors);

    var result = await emailService.SendEmailAsync(request.To, request.Subject, request.Body);
    return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors);
})
.WithName("SendEmail")
.WithOpenApi();

app.Run();

public partial class Program
{}
