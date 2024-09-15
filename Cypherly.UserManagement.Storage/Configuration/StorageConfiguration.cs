using Amazon.S3;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Storage.Services;
using Cypherly.UserManagement.Storage.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cypherly.UserManagement.Storage.Configuration;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAmazonS3>(sp =>

        {
            var minioSettings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
            var credentials = new Amazon.Runtime.BasicAWSCredentials(minioSettings.User, minioSettings.Password);
            return new AmazonS3Client(credentials, new AmazonS3Config
            {
                ServiceURL = minioSettings.Host,
                ForcePathStyle = true
            });
        });

        services.AddScoped<IProfilePictureService, ProfilePictureService>();
        services.AddScoped<IFileValidator, FileValidator>();

        return services;
    }
}