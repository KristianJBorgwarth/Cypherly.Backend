using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace MinimalEmail.API.Configuration;

public static class SmtpClientConfiguration
{
    public static IServiceCollection AddSmtpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        services.AddSingleton<SmtpClient>(s =>
        {
            var smtpSettings = s.GetRequiredService<IOptions<SmtpSettings>>().Value;
            var smtpClient = new SmtpClient();
            smtpClient.Connect(smtpSettings.Host, smtpSettings.Port, SecureSocketOptions.StartTls);
            smtpClient.Authenticate(smtpSettings.Username, smtpSettings.Password);

            return smtpClient;
        });

        return services;
    }
}